using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LogginBehavior<,>));
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!); // connection string desde catalog 
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName); // le estamos diciendo que el Id es UserName
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>(); // inyeccion de dependencia del repositorio 
builder.Services.Decorate<IBasketRepository, CacheBasketRepository>(); // decorador para cache

builder.Services.AddStackExchangeRedisCache(options => // inyeccion de dependencia del cache redis 
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    //options.InstanceName = "Basket"; 
}); 

builder.Services.AddExceptionHandler<CustomExceptionHandler>(); // manejo global de excepciones

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!) // para el health check de la base de datos 
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!); // para el health check del redis

builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapCarter();

app.UseExceptionHandler(options => { }); // middleware global de excepciones

app.UseHealthChecks("/health", 
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }); // endpoint para el health check

app.Run();
