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

        // TODO: once a gRPC service has its .proto and server side, uncomment its client and gateway
        // registrations and remove the matching Unimplemented one.

        // builder.Services.AddGrpcClient<Tagalong.Grpc.Users.UsersClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<IUserGateway, GrpcUserGateway>();
        builder.Services.AddScoped<IUserGateway, UnimplementedUserGateway>();

        // builder.Services.AddGrpcClient<Tagalong.Grpc.Trips.TripsClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<ITripGateway, GrpcTripGateway>();
        builder.Services.AddScoped<ITripGateway, UnimplementedTripGateway>();

        // builder.Services.AddGrpcClient<Tagalong.Grpc.Activities.ActivitiesClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<IActivityGateway, GrpcActivityGateway>();
        builder.Services.AddScoped<IActivityGateway, UnimplementedActivityGateway>();

        // builder.Services.AddGrpcClient<Tagalong.Grpc.Votes.VotesClient>(o => o.Address = new Uri("http://localhost:5001"));
        // builder.Services.AddScoped<IVoteGateway, GrpcVoteGateway>();
        builder.Services.AddScoped<IVoteGateway, UnimplementedVoteGateway>();

        builder.Services.AddScoped<IGreetingService, GreetingService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITripService, TripService>();
        builder.Services.AddScoped<IActivityService, ActivityService>();

        var jwtKey = builder.Configuration[Constants.JwtKey]
                     ?? throw new InvalidOperationException($"Missing configuration value {Constants.JwtKey}.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration[Constants.JwtIssuer],
                    ValidAudience = builder.Configuration[Constants.JwtAudience],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<LiveHub>("/hub");

        app.MapControllers();

        app.Run();
    }
}
