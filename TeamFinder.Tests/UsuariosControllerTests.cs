using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using TeamFinder.Controllers; 
using TeamFinder.Api.Controllers;
using TeamFinder.Api.Data; 

namespace TeamFinder.Tests
{
    public class UsuariosControllerTests
    {
        // -----------------------------------------------------------------
        // Método de ayuda simple para obtener un DbContext en memoria
        // -----------------------------------------------------------------
        private TeamFinderDbContext GetEmptyInMemoryDbContext()
        {
            // Creamos opciones para una base de datos en memoria con un nombre único
            var options = new DbContextOptionsBuilder<TeamFinderDbContext>()
                // Usar un GUID asegura que cada prueba tenga una base de datos fresca.
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}") 
                .Options;
            
            var context = new TeamFinderDbContext(options);
            context.Database.EnsureDeleted(); // Limpia cualquier estado previo
            context.Database.EnsureCreated(); // Crea la DB vacía
            return context;
        }

        // -----------------------------------------------------------------
        // PRUEBA UNITARIA SENCILLA: SteamId Obligatorio
        // -----------------------------------------------------------------
        
        [Fact] // Este atributo es esencial para que xUnit identifique el método como una prueba
        public async Task LoginSteamPost_SteamIdVacio_DebeDevolverBadRequest()
        {
            // 1. Arrange (Preparación)
            
            // Creamos un contexto de DB vacío
            var context = GetEmptyInMemoryDbContext();
            
            // Creamos una instancia del controlador inyectando el contexto falso
            var controller = new UsuariosController(context);
            
            // Creamos el DTO con el campo SteamId vacío o nulo
            var loginDto = new UsuariosController.SteamLoginDto 
            { 
                SteamId = string.Empty // O null 
            };

            // 2. Act (Acción)
            
            // Llamamos al método del controlador que queremos probar
            var resultado = await controller.LoginSteam(loginDto);

            // 3. Assert (Verificación)
            
            // Verificamos que el ActionResult contiene un BadRequestObjectResult (código HTTP 400)
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
            
            // Opcional: Verificamos el mensaje específico de error
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Equal("El SteamId es obligatorio.", badRequest.Value);
        }
    }
}