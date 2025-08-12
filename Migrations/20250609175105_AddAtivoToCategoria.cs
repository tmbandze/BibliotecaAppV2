using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaAppV2.Migrations
{
    /// <inheritdoc />
    public partial class AddAtivoToCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Livros",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Categorias",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Livros");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Categorias");
        }
    }
}
