using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCarter();

// Add services to the container 
var assembly = typeof(Program).Assembly; 
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>)); // tuberia para hacer las validaciones con FluentValidation
    config.AddOpenBehavior(typeof(LogginBehavior<,>)); // logs de información 
});
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

//if (builder.Environment.IsDevelopment()) // siembra de datos de semilla para una tabla o tablas
//    builder.Services.InitializeMartenWith<CatalogInitialData>(); 

builder.Services.AddExceptionHandler<CustomExceptionHandler>(); // excepción personalizada 


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!); // para chequear la salud del servicio de db en postgreSQL en los microservicios

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCarter(); 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler(options => { }); // construir las excepciones 

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }); // endpoint para chequear la salud del servicio

app.Run();
