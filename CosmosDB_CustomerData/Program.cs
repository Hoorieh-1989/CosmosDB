
using CosmosDB_CustomerData.Data.Interfaces;
using CosmosDB_CustomerData.Data.Repos;
using CosmosDB_CustomerData.Data.Services;
using CosmosDB_CustomerData.Endpoints;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(s =>
{
    var connectionString = builder.Configuration["CosmosDb:ConnectionString"];
    return new CosmosClient(connectionString);
});

// Dependency Injection
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kund API",
        Version = "v1",
        Description = "API för hantering av kunder och säljare"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kund API v1");
    });
}

app.MapCustomerEndpoints();

app.Run();