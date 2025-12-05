using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;
using TeamFinder.Api.Services;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamFinder.Tests
{
    public class MatchingServiceTests
    {
        private TeamFinderDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TeamFinderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new TeamFinderDbContext(options);
        }

        [Fact]
        public async Task CalcularPorcentajeMatchAsync_RetornaPuntuacionAlta_CuandoPreferenciasSonIdenticas()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            var usuario1 = new Usuario { Id = 1, Username = "User1", Pais = "Mexico", Correo = "u1@test.com", Contraseña = "123" };
            var usuario2 = new Usuario { Id = 2, Username = "User2", Pais = "Mexico", Correo = "u2@test.com", Contraseña = "123" };

            var pref1 = new PreferenciaMatching
            {
                UsuarioId = 1,
                Usuario = usuario1,
                NivelHabilidad = "Experto",
                EstiloJuego = "Competitivo",
                DiasDisponibles = "FinDeSemana",
                HorarioDisponible = "Noche",
                Idioma = "Español",
                ComunicacionVoz = true,
                RolPreferido = "Atacante",
                MismoPais = true
            };

            var pref2 = new PreferenciaMatching
            {
                UsuarioId = 2,
                Usuario = usuario2,
                NivelHabilidad = "Experto",
                EstiloJuego = "Competitivo",
                DiasDisponibles = "FinDeSemana",
                HorarioDisponible = "Noche",
                Idioma = "Español",
                ComunicacionVoz = true,
                RolPreferido = "Defensor", // Roles diferentes suman puntos en tu lógica
                MismoPais = true
            };

            // Act
            var resultado = await service.CalcularPorcentajeMatchAsync(pref1, pref2);

            // Assert
            Assert.Equal(100, resultado);
        }

        [Fact]
        public async Task CalcularPorcentajeMatchAsync_RetornaMenorPuntuacion_CuandoHayDiferencias()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            var usuario1 = new Usuario { Id = 1, Username = "User1", Pais = "Mexico", Correo = "u1@test.com", Contraseña = "123" };
            var usuario2 = new Usuario { Id = 2, Username = "User2", Pais = "Argentina", Correo = "u2@test.com", Contraseña = "123" };

            var pref1 = new PreferenciaMatching
            {
                Usuario = usuario1,
                NivelHabilidad = "Novato",
                EstiloJuego = "Casual",
                UsuarioId = 1,
                RolPreferido = "Support"
            };

            var pref2 = new PreferenciaMatching
            {
                Usuario = usuario2,
                NivelHabilidad = "Experto", // Diferente
                EstiloJuego = "Competitivo", // Diferente
                UsuarioId = 2,
                RolPreferido = "Support" // Mismo rol (no suma el bonus de roles diferentes)
            };

            // Act
            var resultado = await service.CalcularPorcentajeMatchAsync(pref1, pref2);

            // Assert
            Assert.True(resultado < 100);
        }

        [Fact]
        public async Task CrearMatchAsync_GuardaMatchEnBaseDeDatos_SiUsuariosExisten()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // CORRECCIÓN: Usando las propiedades correctas de tus modelos
            var usuario1 = new Usuario
            {
                Id = 1,
                Username = "User1",   // Antes era Nombre
                Correo = "u1@test.com", // Antes era Email
                Contraseña = "abc",     // Antes era PasswordHash
                Pais = "Mexico",        // Propiedad requerida
                Edad = 20
            };

            var usuario2 = new Usuario
            {
                Id = 2,
                Username = "User2",
                Correo = "u2@test.com",
                Contraseña = "abc",
                Pais = "Mexico",
                Edad = 22
            };

            var juego = new Juego
            {
                Id = 10,
                Nombre = "Valorant",
                Categoria = "Shooter",  // Antes era Genero
                ImagenUrl = "http://img.com" // Requerido por el modelo si no acepta nulls, o buena práctica ponerlo
            };

            context.Usuarios.AddRange(usuario1, usuario2);
            context.Juegos.Add(juego);
            await context.SaveChangesAsync();

            var service = new MatchingService(context);

            // Act
            var match = await service.CrearMatchAsync(usuario1.Id, usuario2.Id, juego.Id, idIniciador: usuario1.Id);

            // Assert
            Assert.NotNull(match);
            Assert.Equal(1, context.Matches.Count());
            Assert.True(match.AceptadoPorUsuario1);
            Assert.False(match.AceptadoPorUsuario2);
            Assert.False(match.MatchConfirmado);
        }

        [Fact]
        public async Task CrearMatchAsync_RetornaNull_SiUsuarioNoExiste()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            // Act: Intentamos crear match con IDs que no existen en la DB en memoria
            var match = await service.CrearMatchAsync(99, 88, 1, 99);

            // Assert
            Assert.Null(match);
            Assert.Equal(0, context.Matches.Count());
        }
    }
}