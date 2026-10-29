using TagalongClient = Tagalong.Grpc.Tagalong.TagalongClient;
using WebApi.Services;
namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthorization();

        app.MapHub<LiveHub>("/hub");

        app.MapControllers();

        app.Run();
    }
}
