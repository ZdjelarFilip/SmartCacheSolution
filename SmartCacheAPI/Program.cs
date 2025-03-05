using Orleans.Configuration;
using SmartCacheAPI.Repositories;
using SmartCacheAPI.Services;
using SmartCacheAPI.Settings;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

var jwtKey = configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new ArgumentNullException("Jwt:Key", "JWT signing key is missing from configuration.");
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

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

// Register Services and Controllers
builder.Services.AddSingleton<IBreachedEmailStorage, AzureBreachedEmailStorage>();
builder.Services.AddScoped<EmailBreachService>();
builder.Services.Configure<AzureSettings>(configuration.GetSection("AzureStorage"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
