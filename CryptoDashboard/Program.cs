using CryptoDashboard.Context;
using CryptoDashboard.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var environment = builder.Environment.EnvironmentName;

var connectionString = environment == "Production"
    ? Environment.GetEnvironmentVariable("CONNECTION_STRING")
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

// Add CORS Service

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Hangfire

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1;
    options.ShutdownTimeout = TimeSpan.FromMinutes(5);
});


builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(connectionString, new Hangfire.PostgreSql.PostgreSqlStorageOptions
    {
        SchemaName = "HangFire",
        QueuePollInterval = TimeSpan.FromMinutes(10),
        DistributedLockTimeout = TimeSpan.FromMinutes(3)
    });
});


builder.Services.AddScoped<CryptoJobService>();

// 

var app = builder.Build();

// Enable CORS

app.UseCors("AllowAll");

// middleware

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Controllers mapping
app.MapControllers();

//Hangfire Dashboard
//app.UseHangfireDashboard("/hangfire", new DashboardOptions
//                                    {
//                                        Authorization = new[] { new AllowAllDashboardAuthorizationFilter() }
//                                    });

//Create hangfire job
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate(
        "FetchCryptoPrices",
        () => scope.ServiceProvider.GetRequiredService<CryptoJobService>().FetchAndStorePrices(),
        "0 0 * * *"
    );
}


app.Run();
