using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Data;
using Ordering.Infrastructure.Data.Interceptors;

namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");

            // Add services to the container 
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>(); 

            // Add services to the container 
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                // Esta es la clase que auditara nuestros cambios con Entity Framework Core 
                // y de esta manera poder saber como se dan los cambios 
                // Tambien los elementos de esto estan en Entity 
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>()); 
                options.UseSqlServer(connectionString); 
            });

            // injectar la interfaz de aplication aqui en infrastructure 
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>(); 

            return services; 
        }
    }
}
