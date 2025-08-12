using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaAppV2.Migrations
{
    /// <inheritdoc />
    public partial class AddAtivoToMembro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Membros",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Membros");
        }
    }
}
