using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HeladeriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreandoTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Helados");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Ingredientes",
                newName: "nombreIngrediente");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredientes_Nombre",
                table: "Ingredientes",
                newName: "IX_Ingredientes_nombreIngrediente");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Estados",
                newName: "nombreEstado");

            migrationBuilder.RenameIndex(
                name: "IX_Estados_Nombre",
                table: "Estados",
                newName: "IX_Estados_nombreEstado");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Categorias",
                newName: "nombreCategoria");

            migrationBuilder.RenameIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                newName: "IX_Categorias_nombreCategoria");

            migrationBuilder.AddColumn<string>(
                name: "nombreHelado",
                table: "Helados",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Helados",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "EstadoId", "FechaCreacion", "IsArtesanal", "Precio", "nombreHelado" },
                values: new object[,]
                {
                    { 1, 2, "Helado de menta con chips de chocolate", 1, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1500.0, "Menta Granizada" },
                    { 2, 3, "Helado de sambayon", 1, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2500.0, "Sambayon" },
                    { 3, 1, "Helado de pastel de lima", 3, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 1000.0, "Pastel de lima" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Helados_nombreHelado",
                table: "Helados",
                column: "nombreHelado",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Helados_nombreHelado",
                table: "Helados");

            migrationBuilder.DeleteData(
                table: "Helados",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Helados",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Helados",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "nombreHelado",
                table: "Helados");

            migrationBuilder.RenameColumn(
                name: "nombreIngrediente",
                table: "Ingredientes",
                newName: "Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredientes_nombreIngrediente",
                table: "Ingredientes",
                newName: "IX_Ingredientes_Nombre");

            migrationBuilder.RenameColumn(
                name: "nombreEstado",
                table: "Estados",
                newName: "Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Estados_nombreEstado",
                table: "Estados",
                newName: "IX_Estados_Nombre");

            migrationBuilder.RenameColumn(
                name: "nombreCategoria",
                table: "Categorias",
                newName: "Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Categorias_nombreCategoria",
                table: "Categorias",
                newName: "IX_Categorias_Nombre");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Helados",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
