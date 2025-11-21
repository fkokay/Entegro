using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addshipmentShippingIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingIntegrationId",
                table: "Shipment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_ShippingIntegrationId",
                table: "Shipment",
                column: "ShippingIntegrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_IntegrationSystem_ShippingIntegrationId",
                table: "Shipment",
                column: "ShippingIntegrationId",
                principalTable: "IntegrationSystem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_IntegrationSystem_ShippingIntegrationId",
                table: "Shipment");

            migrationBuilder.DropIndex(
                name: "IX_Shipment_ShippingIntegrationId",
                table: "Shipment");

            migrationBuilder.DropColumn(
                name: "ShippingIntegrationId",
                table: "Shipment");
        }
    }
}
