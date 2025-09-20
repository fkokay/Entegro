using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductEditMinStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinStockQuantity",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinStockQuantity",
                table: "Product");
        }
    }
}
