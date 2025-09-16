using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShipmentItemIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItem_OrderItemId",
                table: "ShipmentItem",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentItem_OrderItem_OrderItemId",
                table: "ShipmentItem",
                column: "OrderItemId",
                principalTable: "OrderItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentItem_OrderItem_OrderItemId",
                table: "ShipmentItem");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentItem_OrderItemId",
                table: "ShipmentItem");
        }
    }
}
