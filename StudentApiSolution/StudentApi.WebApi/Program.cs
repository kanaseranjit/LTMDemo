using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StudentApi.Application.Interfaces;
using StudentApi.Infrastructure.Data;
using StudentApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("c:Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

//builder.Host.UseSerilog(); // Replace default logger with Serilog

builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
//builder.Services.AddSingleton<Serilog.ILogger, StudentRepository>();
builder.Services.AddScoped<IAdmissionRepository, AdmissionRepository>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000", // React dev server
                "https://reactstudentapp-c6dah9bwhthnbgfg.austriaeast-01.azurewebsites.net" // Production React app
            )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable CORS BEFORE controllers
app.UseCors("AllowReactApp");

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
