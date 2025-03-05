using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TraditionalAPI.Data;
using TraditionalAPI.Repositories;
using TraditionalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Database Context (SQL Server)
builder.Services.AddDbContext<BreachDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register Services
builder.Services.AddScoped<IBreachedEmailStorage, BreachedEmailStorage>();
builder.Services.AddScoped<EmailBreachService>();

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartCache API (Traditional)", Version = "v1" });
});

var app = builder.Build();

// Configure Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartCache API (Traditional) v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();