using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditProductIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductVariantAttributeCombinationId",
                table: "ProductIntegration",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductIntegration_ProductVariantAttributeCombinationId",
                table: "ProductIntegration",
                column: "ProductVariantAttributeCombinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIntegration_ProductVariantAttributeCombination_ProductVariantAttributeCombinationId",
                table: "ProductIntegration",
                column: "ProductVariantAttributeCombinationId",
                principalTable: "ProductVariantAttributeCombination",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductIntegration_ProductVariantAttributeCombination_ProductVariantAttributeCombinationId",
                table: "ProductIntegration");

            migrationBuilder.DropIndex(
                name: "IX_ProductIntegration_ProductVariantAttributeCombinationId",
                table: "ProductIntegration");

            migrationBuilder.DropColumn(
                name: "ProductVariantAttributeCombinationId",
                table: "ProductIntegration");
        }
    }
}
