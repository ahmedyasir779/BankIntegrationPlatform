using BankIntegration.Api.Application.Interfaces;
using BankIntegration.Api.Application.Services;
using BankIntegration.Api.Infrastructure.Configurations;
using BankIntegration.Api.Infrastructure.Security;
using Microsoft.Extensions.Options;
using BankIntegration.Api.Infrastructure.External.Adapters;
using BankIntegration.Api.Infrastructure.External.AdapterRegistry;
using BankIntegration.Api.Middleware;
using BankIntegration.Api.Application.Common;
using BankIntegration.Api.Common;
using BankIntegration.Api.Infrastructure.External.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
// builder.Services.AddScoped<IBankHttpClient, BankHttpClient>();
builder.Services.AddHttpClient<IBankHttpClient, BankHttpClient>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApiResponseFactory, ApiResponseFactory>();
builder.Services.AddScoped<IRequestContextAccessor, RequestContextAccessor>();

builder.Services.AddScoped<IRequestContextAccessor,
                           RequestContextAccessor>();


// ===========================================
// Configure JWT Bearer authentication.
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>()!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();
// ===========================================

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

// JWT Bearer authentication.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

