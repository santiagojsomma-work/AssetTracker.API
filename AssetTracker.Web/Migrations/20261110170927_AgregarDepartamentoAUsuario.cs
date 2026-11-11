using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTracker.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDepartamentoAUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Departamento",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departamento",
                table: "AspNetUsers");
        }
    }
}
