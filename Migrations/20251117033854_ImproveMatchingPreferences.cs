using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamFinder.Migrations
{
    /// <inheritdoc />
    public partial class ImproveMatchingPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SoloMicrófono",
                table: "PreferenciasMatching",
                newName: "MismoPais");

            migrationBuilder.RenameColumn(
                name: "EdadMinima",
                table: "PreferenciasMatching",
                newName: "HorasPorSesion");

            migrationBuilder.RenameColumn(
                name: "EdadMaxima",
                table: "PreferenciasMatching",
                newName: "HorasEnJuego");

            migrationBuilder.RenameColumn(
                name: "Disponibilidad",
                table: "PreferenciasMatching",
                newName: "RolPreferido");

            migrationBuilder.AddColumn<bool>(
                name: "ComunicacionVoz",
                table: "PreferenciasMatching",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DiasDisponibles",
                table: "PreferenciasMatching",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EdadMaximaPreferida",
                table: "PreferenciasMatching",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EdadMinimaPreferida",
                table: "PreferenciasMatching",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EstiloJuego",
                table: "PreferenciasMatching",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "PreferenciasMatching",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "HorarioDisponible",
                table: "PreferenciasMatching",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MicrofonoRequerido",
                table: "PreferenciasMatching",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaisPreferido",
                table: "PreferenciasMatching",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RangoCompetitivo",
                table: "PreferenciasMatching",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SteamAppId",
                table: "Juegos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComunicacionVoz",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "DiasDisponibles",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "EdadMaximaPreferida",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "EdadMinimaPreferida",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "EstiloJuego",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "HorarioDisponible",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "MicrofonoRequerido",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "PaisPreferido",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "RangoCompetitivo",
                table: "PreferenciasMatching");

            migrationBuilder.DropColumn(
                name: "SteamAppId",
                table: "Juegos");

            migrationBuilder.RenameColumn(
                name: "RolPreferido",
                table: "PreferenciasMatching",
                newName: "Disponibilidad");

            migrationBuilder.RenameColumn(
                name: "MismoPais",
                table: "PreferenciasMatching",
                newName: "SoloMicrófono");

            migrationBuilder.RenameColumn(
                name: "HorasPorSesion",
                table: "PreferenciasMatching",
                newName: "EdadMinima");

            migrationBuilder.RenameColumn(
                name: "HorasEnJuego",
                table: "PreferenciasMatching",
                newName: "EdadMaxima");
        }
    }
}
