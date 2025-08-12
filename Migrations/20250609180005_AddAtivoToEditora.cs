using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaAppV2.Migrations
{
    /// <inheritdoc />
    public partial class AddAtivoToEditora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Reservas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Emprestimos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Editoras",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Emprestimos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Editoras");
        }
    }
}
