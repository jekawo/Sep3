using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TagalongClient = Tagalong.Grpc.Tagalong.TagalongClient;
using WebApi.Gateways;
using WebApi.Gateways.Interfaces;
using WebApi.Infrastructure;
using WebApi.Services;
using WebApi.Services.Interfaces;
namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        // The Validate extensions report missing fields, so the implicit [Required] check is turned off.
        builder.Services.AddControllers(options =>
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<NotImplementedExceptionHandler>();

        builder.Services.AddSignalR();

        // The Blazor client runs on a different origin, so SignalR needs CORS with credentials.
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy
                .WithOrigins("https://localhost:7192", "http://localhost:5103")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        builder.Services.AddGrpcClient<TagalongClient>(o =>
        {
            o.Address = new Uri("http://localhost:5001");
        });

        // Services only know the gateway interfaces, so changing the registration here swaps the
        // data source without touching them. If one interface is registered twice, the last one wins.
        // TODO: once a gRPC service has its .proto and server side, uncomment its client and gateway
        // registrations and remove the matching Unimplemented one.

        // builder.Services.AddGrpcClient<Tagalong.Grpc.Users.UsersClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<IUserGateway, GrpcUserGateway>();
        builder.Services.AddScoped<IUserGateway, UnimplementedUserGateway>();

        // builder.Services.AddGrpcClient<Tagalong.Grpc.RefreshTokens.RefreshTokensClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<IRefreshTokenGateway, GrpcRefreshTokenGateway>();
        builder.Services.AddScoped<IRefreshTokenGateway, UnimplementedRefreshTokenGateway>();

        // builder.Services.AddGrpcClient<Tagalong.Grpc.Trips.TripsClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<ITripGateway, GrpcTripGateway>();
        builder.Services.AddScoped<ITripGateway, UnimplementedTripGateway>();

        // builder.Services.AddGrpcClient<Tagalong.Grpc.Activities.ActivitiesClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<IActivityGateway, GrpcActivityGateway>();
        builder.Services.AddScoped<IActivityGateway, UnimplementedActivityGateway>();

        // builder.Services.AddGrpcClient<Tagalong.Grpc.Votes.VotesClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<IVoteGateway, GrpcVoteGateway>();
        builder.Services.AddScoped<IVoteGateway, UnimplementedVoteGateway>();

        // Scoped means one instance per HTTP request.
        builder.Services.AddScoped<IGreetingService, GreetingService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITripService, TripService>();
        builder.Services.AddScoped<IActivityService, ActivityService>();

        var jwtKey = builder.Configuration[Constants.JwtKey]
                     ?? throw new InvalidOperationException($"Missing configuration value {Constants.JwtKey}.");

        // How incoming "Authorization: Bearer <token>" headers are checked. The issuer, audience and
        // key must match the ones AuthenticationService uses to create the token.
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    // Tokens are accepted this long after expiry to allow for clock differences.
                    // The default is 5 minutes, which is a lot for a 15 minute token.
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration[Constants.JwtIssuer],
                    ValidAudience = builder.Configuration[Constants.JwtAudience],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };

                // Browsers cannot set the Authorization header on WebSocket connections, so SignalR
                // clients send the token as the "access_token" query parameter instead. It is only
                // accepted for the hub, because URLs are more likely than headers to end up in logs.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments(Constants.HubPath))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        // Requests pass through the middleware in this order. The exception handler is early
        // so it can catch exceptions thrown by everything after it.
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        // Authentication finds out who the caller is, authorization decides what they may do.
        // The order matters, because [Authorize] needs to know the user first.
        app.UseAuthentication();
        app.UseAuthorization();

        // The token is checked when connecting. This option also closes the connection when
        // the token expires, instead of letting it stay open indefinitely.
        app.MapHub<LiveHub>(Constants.HubPath, options => options.CloseOnAuthenticationExpiration = true);

        app.MapControllers();

        app.Run();
    }
}
