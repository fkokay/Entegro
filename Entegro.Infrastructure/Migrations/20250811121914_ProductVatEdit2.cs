using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductVatEdit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VatInc",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "VatInc",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "Product");
        }
    }
}
