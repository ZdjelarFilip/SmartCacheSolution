using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Orleans.Configuration;
using SmartCacheAPI.Repositories;
using SmartCacheAPI.Services;
using SmartCacheAPI.Settings;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigurationManager configuration = builder.Configuration;

        // Register Orleans Silo
        builder.Host.UseOrleans(siloBuilder =>
        {
            siloBuilder.UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "orleans";
                })
                .AddAzureBlobGrainStorage("nomnioresource", options =>
                {
                    options.BlobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(configuration["AzureStorage:ConnectionString"]);
                    options.ContainerName = configuration["AzureStorage:ContainerName"];
                })
                .UseInMemoryReminderService();
        });

        // Register Logging
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        // Register Services and Controllers
        builder.Services.AddSingleton<IBreachedEmailStorage, AzureBreachedEmailStorage>();
        builder.Services.AddScoped<EmailBreachService>();
        builder.Services.Configure<AzureSettings>(configuration.GetSection("AzureStorage"));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}