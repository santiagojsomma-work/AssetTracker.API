using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class CodigoInternoYEstados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoInterno",
                table: "Activos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activos_CodigoInterno",
                table: "Activos",
                column: "CodigoInterno",
                unique: true,
                filter: "CodigoInterno IS NOT NULL");

            migrationBuilder.Sql("UPDATE Activos SET CodigoInterno = 'ACT-' || printf('%04d', Id) WHERE CodigoInterno IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Activos_CodigoInterno",
                table: "Activos");

            migrationBuilder.DropColumn(
                name: "CodigoInterno",
                table: "Activos");
        }
    }
}
