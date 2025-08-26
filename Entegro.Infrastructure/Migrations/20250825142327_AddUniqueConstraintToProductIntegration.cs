using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToProductIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductIntegration_IntegrationSystemId",
                table: "ProductIntegration");

            migrationBuilder.AlterColumn<string>(
                name: "IntegrationCode",
                table: "ProductIntegration",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductIntegration_IntegrationSystemId_IntegrationCode",
                table: "ProductIntegration",
                columns: new[] { "IntegrationSystemId", "IntegrationCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductIntegration_IntegrationSystemId_IntegrationCode",
                table: "ProductIntegration");

            migrationBuilder.AlterColumn<string>(
                name: "IntegrationCode",
                table: "ProductIntegration",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductIntegration_IntegrationSystemId",
                table: "ProductIntegration",
                column: "IntegrationSystemId");
        }
    }
}
