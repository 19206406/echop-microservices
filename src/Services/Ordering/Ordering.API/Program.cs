using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add serveces to the container 

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(); 

var app = builder.Build();

// Configure the HTTP request pipeline 

// Automigración de la base de datos a los entornos de docker 

// esto ayuda a que cada vez que se hagan cambíos en la db estos se guarden automaticamente 
// sin la necesidad de ejecutarlos manualmente pero solo en entorno de desarrollo en producción
// debe de ser otra cosa 
app.UseApiServices(); 
if (app.Environment.IsDevelopment()) 
{
    await app.InitialseDatabaseAsync(); 
}

app.Run();

