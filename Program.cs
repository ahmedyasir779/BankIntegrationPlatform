using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Application.Services;
using BankIntegrationPlatform.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using BankIntegrationPlatform.Infrastructure.External.Adapters;
using BankIntegrationPlatform.Infrastructure.External.AdapterRegistry;
using BankIntegrationPlatform.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BankOptions>(builder.Configuration.GetSection("BankOptions"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBankAdapter, SNBAdapter>();
builder.Services.AddScoped<IBankAdapter, RiyadAdapter>();
builder.Services.AddScoped<IBankAdapter, AlRajhiAdapter>();
builder.Services.AddScoped<IBankAdapter, MockBankAdapter>();

builder.Services.AddScoped<AdapterRegistry>();
builder.Services.AddScoped<IBankService, BankService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Register our custom middleware
app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();

