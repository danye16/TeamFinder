using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TeamFinder.Api.Data;

namespace TeamFinder.Tests
{
    public class CustomWebApplicationFactory<TProgram> 
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // --------------------------------------------------------
                // PASO 1: ELIMINAR LA CONFIGURACIÓN EXISTENTE (SQL SERVER)
                // --------------------------------------------------------
                
                // Buscamos las opciones del DbContext (la configuración)
                var descriptorOptions = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TeamFinderDbContext>));

                if (descriptorOptions != null)
                {
                    services.Remove(descriptorOptions);
                }

                // Buscamos el DbContext en sí (la clase) - ¡ESTO ES LO QUE FALTABA!
                var descriptorContext = services.SingleOrDefault(
                    d => d.ServiceType == typeof(TeamFinderDbContext));

                if (descriptorContext != null)
                {
                    services.Remove(descriptorContext);
                }

                // --------------------------------------------------------
                // PASO 2: AGREGAR LA NUEVA BASE DE DATOS (EN MEMORIA)
                // --------------------------------------------------------
                
                services.AddDbContext<TeamFinderDbContext>(options =>
                {
                    // Usamos un nombre único para asegurar aislamiento
                    options.UseInMemoryDatabase("InMemoryDbForIntegration");
                });

                // --------------------------------------------------------
                // PASO 3: ASEGURAR QUE SE CREE LA BASE DE DATOS
                // --------------------------------------------------------
                
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<TeamFinderDbContext>();
                    
                    // Esto crea la estructura de tablas en la memoria
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}