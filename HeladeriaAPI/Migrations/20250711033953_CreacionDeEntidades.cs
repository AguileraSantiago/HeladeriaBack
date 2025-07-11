using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HeladeriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreacionDeEntidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreCategoria = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreEstado = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreIngrediente = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Helados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreHelado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<double>(type: "float", nullable: false),
                    IsArtesanal = table.Column<bool>(type: "bit", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Helados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Helados_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Helados_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredienteHelado",
                columns: table => new
                {
                    HeladoId = table.Column<int>(type: "int", nullable: false),
                    IngredienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredienteHelado", x => new { x.HeladoId, x.IngredienteId });
                    table.ForeignKey(
                        name: "FK_IngredienteHelado_Helados_HeladoId",
                        column: x => x.HeladoId,
                        principalTable: "Helados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredienteHelado_Ingredientes_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "Ingredientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "nombreCategoria" },
                values: new object[,]
                {
                    { 1, "Frutales" },
                    { 2, "Granizados" },
                    { 3, "Chocolate" },
                    { 4, "Al agua" },
                    { 5, "Clasicos" }
                });

            migrationBuilder.InsertData(
                table: "Estados",
                columns: new[] { "Id", "nombreEstado" },
                values: new object[,]
                {
                    { 1, "Disponible" },
                    { 2, "Pendiente" },
                    { 3, "No Disponible" }
                });

            migrationBuilder.InsertData(
                table: "Ingredientes",
                columns: new[] { "Id", "nombreIngrediente" },
                values: new object[,]
                {
                    { 1, "Leche" },
                    { 2, "Azucar" },
                    { 3, "Alcohol" },
                    { 4, "Chocolate" },
                    { 5, "Crema" },
                    { 6, "Huevo" },
                    { 7, "Limon" },
                    { 8, "Esencia de menta" }
                });

            migrationBuilder.InsertData(
                table: "Helados",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "EstadoId", "FechaCreacion", "IsArtesanal", "Precio", "nombreHelado" },
                values: new object[,]
                {
                    { 1, 2, "Helado fresco y cremoso con intenso sabor a menta natural, combinado con trocitos de chocolate semiamargo", 1, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1500.0, "Menta Granizada" },
                    { 2, 5, "Cremoso helado con una textura suave y un sabor inconfundible que combina dulzura y un toque alcohólico delicado ", 1, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2500.0, "Sambayon" },
                    { 3, 1, "Refrescante y dulce a la vez, este helado recrea el sabor del famoso postre Lemon Pie", 3, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 1000.0, "Pastel de lima" }
                });

            migrationBuilder.InsertData(
                table: "IngredienteHelado",
                columns: new[] { "HeladoId", "IngredienteId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 4 },
                    { 1, 5 },
                    { 1, 8 },
                    { 2, 1 },
                    { 2, 2 },
                    { 2, 3 },
                    { 2, 5 },
                    { 2, 6 },
                    { 3, 1 },
                    { 3, 2 },
                    { 3, 5 },
                    { 3, 6 },
                    { 3, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_nombreCategoria",
                table: "Categorias",
                column: "nombreCategoria",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estados_nombreEstado",
                table: "Estados",
                column: "nombreEstado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Helados_CategoriaId",
                table: "Helados",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Helados_EstadoId",
                table: "Helados",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Helados_nombreHelado",
                table: "Helados",
                column: "nombreHelado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredienteHelado_IngredienteId",
                table: "IngredienteHelado",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredientes_nombreIngrediente",
                table: "Ingredientes",
                column: "nombreIngrediente",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredienteHelado");

            migrationBuilder.DropTable(
                name: "Helados");

            migrationBuilder.DropTable(
                name: "Ingredientes");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Estados");
        }
    }
}
