using CryptoDashboard.Context; 
using CryptoDashboard.Services; 
using Hangfire; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var connectionString = builder.Configuration.GetConnectionString("Connection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Hangfire

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

builder.Services.AddScoped<CryptoJobService>();

// 

var app = builder.Build();

// middleware

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

// Swagger
if (app.Environment.IsDevelopment())
{
}
    app.UseSwagger();
    app.UseSwaggerUI();

// Controllers mapping
app.MapControllers();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllDashboardAuthorizationFilter() }
});

// Create hangfire job
RecurringJob.AddOrUpdate<CryptoJobService>(
    "FetchCryptoPrices", 
    service => service.FetchAndStorePrices(), 
    "*/30 * * * *" 
);

app.Run();
