using System.Net;
using System.Threading.Tasks;
using Xunit;
using TeamFinder.Tests;
using TeamFinder.Api;

namespace TeamFinder.Tests.Integration
{
    public class UsuariosIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UsuariosIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        //[Fact]
        //public async Task Get_MostrarUsuarios_ReturnsOk()
        //{
        //    // Act: Llamamos al endpoint que lista usuarios (definido en tu UsuariosController)
        //    var response = await _client.GetAsync("/api/Usuarios/MostrarUsuarios");

        //    // Assert: Verificamos que la respuesta sea exitosa (Código 200-299)
        //    // Si la app "truena" al arrancar, esta línea fallará indicando el error real.
        //    response.EnsureSuccessStatusCode();

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}
    }
}