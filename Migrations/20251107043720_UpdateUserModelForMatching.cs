using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamFinder.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserModelForMatching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seguimientos_UsuarioId",
                table: "Seguimientos");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "EventosGaming",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seguimientos_UsuarioId_SeguidoId",
                table: "Seguimientos",
                columns: new[] { "UsuarioId", "SeguidoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventosGaming_UsuarioId",
                table: "EventosGaming",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventosGaming_Usuarios_UsuarioId",
                table: "EventosGaming",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventosGaming_Usuarios_UsuarioId",
                table: "EventosGaming");

            migrationBuilder.DropIndex(
                name: "IX_Seguimientos_UsuarioId_SeguidoId",
                table: "Seguimientos");

            migrationBuilder.DropIndex(
                name: "IX_EventosGaming_UsuarioId",
                table: "EventosGaming");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "EventosGaming");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimientos_UsuarioId",
                table: "Seguimientos",
                column: "UsuarioId");
        }
    }
}
