using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaAppV2.Migrations
{
    /// <inheritdoc />
    public partial class AddDescricaoEDataCadastro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            

            migrationBuilder.CreateIndex(
                name: "IX_Livros_EditoraId",
                table: "Livros",
                column: "EditoraId");

            migrationBuilder.CreateIndex(
                name: "IX_LivroAutores_AutorId",
                table: "LivroAutores",
                column: "AutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Livros_Editoras_EditoraId",
                table: "Livros",
                column: "EditoraId",
                principalTable: "Editoras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livros_Editoras_EditoraId",
                table: "Livros");

            migrationBuilder.DropTable(
                name: "LivroAutores");

            migrationBuilder.DropIndex(
                name: "IX_Livros_EditoraId",
                table: "Livros");

            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Livros");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Livros");

            migrationBuilder.DropColumn(
                name: "EditoraId",
                table: "Livros");

            migrationBuilder.AddColumn<string>(
                name: "Autor",
                table: "Livros",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Editora",
                table: "Livros",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
