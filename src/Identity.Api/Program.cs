using Identity.Api.Authentication.Services;
using Identity.Api.Infrastructure.Security;
using Identity.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Identity.Api.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// builder.Services.AddSingleton<IClientRegistry, InMemoryClientRegistry>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddScoped<IClientValidationService, ClientValidationService>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// DB
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();