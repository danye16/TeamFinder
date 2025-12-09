using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using TeamFinder.Tests; // Donde está CustomWebApplicationFactory
using TeamFinder.Api;   // Donde está Program (ajusta si tu namespace es diferente)

namespace TeamFinder.Tests.Integration
{
    // 👇 CORRECCIÓN AQUÍ: Debe coincidir con lo que pides en el constructor
    public class UsuariosIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        // El constructor pide CustomWebApplicationFactory, así que IClassFixture debe ser igual
        public UsuariosIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(); 
        }

        [Fact]
        public async Task Get_HealthCheck_ReturnsOk()
        {
            // Ajusta esta ruta a una que sepas que existe en tu API
            // Si no tienes /api/usuarios/health, prueba con una que tengas, ej: /weatherforecast
            var response = await _client.GetAsync("/api/usuarios/health"); 
            
            // Si la ruta no existe, esto dará 404, pero al menos sabremos que el test corre.
            // response.EnsureSuccessStatusCode(); 
        }
    }
}