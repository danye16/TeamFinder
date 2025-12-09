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
                // PASO 1: LIMPIEZA TOTAL DE SQL SERVER
                // --------------------------------------------------------

                // Buscamos TODOS los servicios de base de datos, incluyendo la opción genérica
                var descriptorsToRemove = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<TeamFinderDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(TeamFinderDbContext))
                    .ToList();

                // Los eliminamos todos para dejar el contenedor limpio
                foreach (var descriptor in descriptorsToRemove)
                {
                    services.Remove(descriptor);
                }

                // --------------------------------------------------------
                // PASO 2: AGREGAR IN-MEMORY DB
                // --------------------------------------------------------

                services.AddDbContext<TeamFinderDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForIntegration");
                    // Ignoramos advertencias de transacciones
                    options.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
                });

                // --------------------------------------------------------
                // PASO 3: INICIALIZAR LA BASE DE DATOS
                // --------------------------------------------------------

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<TeamFinderDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}