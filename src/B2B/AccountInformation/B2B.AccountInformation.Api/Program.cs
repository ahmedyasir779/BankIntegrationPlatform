// using B2B.AccountInformation.Core.DependencyInjection;
using B2B.AccountInformation.Infrastructure.DependencyInjection;
// using B2B.AccountInformation.Persistence.DependencyInjection;
// using B2B.AccountInformation.Api.DependencyInjection;
using B2B.AccountInformation.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddApplicationServices(builder.Configuration);
//builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
// builder.Services.AddInfrastructure();
// builder.Services.AddPersistence();
// builder.Services.AddAuthenticationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();