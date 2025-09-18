using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditOrderIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntegrationSystemId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_IntegrationSystemId",
                table: "Order",
                column: "IntegrationSystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_IntegrationSystem_IntegrationSystemId",
                table: "Order",
                column: "IntegrationSystemId",
                principalTable: "IntegrationSystem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_IntegrationSystem_IntegrationSystemId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_IntegrationSystemId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IntegrationSystemId",
                table: "Order");
        }
    }
}
