using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamFinder.Migrations
{
    /// <inheritdoc />
    public partial class CambiosParaMatching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Edad",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EstiloJuego",
                table: "Usuarios",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "Usuarios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EventosGaming",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    JuegoId = table.Column<int>(type: "int", nullable: false),
                    OrganizadorId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxParticipantes = table.Column<int>(type: "int", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsPublico = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JuegoId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosGaming", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosGaming_Juegos_JuegoId",
                        column: x => x.JuegoId,
                        principalTable: "Juegos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventosGaming_Juegos_JuegoId1",
                        column: x => x.JuegoId1,
                        principalTable: "Juegos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventosGaming_Usuarios_OrganizadorId",
                        column: x => x.OrganizadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Insignias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IconoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequisitoPuntuacion = table.Column<int>(type: "int", nullable: false),
                    RequisitoCantidadPartidas = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insignias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usuario1Id = table.Column<int>(type: "int", nullable: false),
                    Usuario2Id = table.Column<int>(type: "int", nullable: false),
                    JuegoId = table.Column<int>(type: "int", nullable: false),
                    FechaMatch = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AceptadoPorUsuario1 = table.Column<bool>(type: "bit", nullable: false),
                    AceptadoPorUsuario2 = table.Column<bool>(type: "bit", nullable: false),
                    MatchConfirmado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Juegos_JuegoId",
                        column: x => x.JuegoId,
                        principalTable: "Juegos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Usuarios_Usuario1Id",
                        column: x => x.Usuario1Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Usuarios_Usuario2Id",
                        column: x => x.Usuario2Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreferenciasMatching",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    JuegoId = table.Column<int>(type: "int", nullable: false),
                    NivelHabilidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disponibilidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Idioma = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EdadMinima = table.Column<int>(type: "int", nullable: false),
                    EdadMaxima = table.Column<int>(type: "int", nullable: false),
                    SoloMicrófono = table.Column<bool>(type: "bit", nullable: false),
                    NotasAdicionales = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JuegoId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenciasMatching", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreferenciasMatching_Juegos_JuegoId",
                        column: x => x.JuegoId,
                        principalTable: "Juegos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreferenciasMatching_Juegos_JuegoId1",
                        column: x => x.JuegoId1,
                        principalTable: "Juegos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PreferenciasMatching_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reputaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EvaluadorId = table.Column<int>(type: "int", nullable: false),
                    Puntuacion = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reputaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reputaciones_Usuarios_EvaluadorId",
                        column: x => x.EvaluadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reputaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventoParticipantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Confirmado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoParticipantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoParticipantes_EventosGaming_EventoId",
                        column: x => x.EventoId,
                        principalTable: "EventosGaming",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoParticipantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioInsignias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    InsigniaId = table.Column<int>(type: "int", nullable: false),
                    FechaObtencion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioInsignias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioInsignias_Insignias_InsigniaId",
                        column: x => x.InsigniaId,
                        principalTable: "Insignias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioInsignias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoParticipantes_EventoId_UsuarioId",
                table: "EventoParticipantes",
                columns: new[] { "EventoId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventoParticipantes_UsuarioId",
                table: "EventoParticipantes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosGaming_JuegoId",
                table: "EventosGaming",
                column: "JuegoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosGaming_JuegoId1",
                table: "EventosGaming",
                column: "JuegoId1");

            migrationBuilder.CreateIndex(
                name: "IX_EventosGaming_OrganizadorId",
                table: "EventosGaming",
                column: "OrganizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_JuegoId",
                table: "Matches",
                column: "JuegoId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Usuario1Id",
                table: "Matches",
                column: "Usuario1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Usuario2Id",
                table: "Matches",
                column: "Usuario2Id");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenciasMatching_JuegoId",
                table: "PreferenciasMatching",
                column: "JuegoId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenciasMatching_JuegoId1",
                table: "PreferenciasMatching",
                column: "JuegoId1");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenciasMatching_UsuarioId",
                table: "PreferenciasMatching",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Reputaciones_EvaluadorId",
                table: "Reputaciones",
                column: "EvaluadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reputaciones_UsuarioId_EvaluadorId",
                table: "Reputaciones",
                columns: new[] { "UsuarioId", "EvaluadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioInsignias_InsigniaId",
                table: "UsuarioInsignias",
                column: "InsigniaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioInsignias_UsuarioId",
                table: "UsuarioInsignias",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoParticipantes");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "PreferenciasMatching");

            migrationBuilder.DropTable(
                name: "Reputaciones");

            migrationBuilder.DropTable(
                name: "UsuarioInsignias");

            migrationBuilder.DropTable(
                name: "EventosGaming");

            migrationBuilder.DropTable(
                name: "Insignias");

            migrationBuilder.DropColumn(
                name: "Edad",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EstiloJuego",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Pais",
                table: "Usuarios");
        }
    }
}
