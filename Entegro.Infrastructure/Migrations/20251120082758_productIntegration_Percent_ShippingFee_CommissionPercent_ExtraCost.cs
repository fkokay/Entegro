using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class productIntegration_Percent_ShippingFee_CommissionPercent_ExtraCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercent",
                table: "ProductIntegration",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraCost",
                table: "ProductIntegration",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Percent",
                table: "ProductIntegration",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFee",
                table: "ProductIntegration",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionPercent",
                table: "ProductIntegration");

            migrationBuilder.DropColumn(
                name: "ExtraCost",
                table: "ProductIntegration");

            migrationBuilder.DropColumn(
                name: "Percent",
                table: "ProductIntegration");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "ProductIntegration");
        }
    }
}
