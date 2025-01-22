using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);


// Create Serilog logger 

// Using RoolinFile with Rolling packahe to write log to Azure logs. 

/*Serilog.Core.Logger logger = new LoggerConfiguration()
   .ReadFrom.Configuration(builder.Configuration)
   .Enrich.FromLogContext()
   .WriteTo.RollingFile("../logs/mydemoissuer.log")
   .CreateLogger();*/




var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read settings from appsettings.json
    .Enrich.FromLogContext()                       // Enrich logs with contextual data
    .WriteTo.Async(a => a.File(
        path: "../logs/mydemoissuer-.log",         // Path with rolling file support
        rollingInterval: RollingInterval.Day,     // Roll logs daily
        retainedFileCountLimit: 7,                // Keep logs for 7 days
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{Exception}" // Custom log format
    ))
    .CreateLogger();




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add serliog to builder 

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
