using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Order",
                newName: "OrderTotal");

            migrationBuilder.RenameColumn(
                name: "OrderNo",
                table: "Order",
                newName: "VatNumber");

            migrationBuilder.AddColumn<int>(
                name: "BillingAddressId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyRate",
                table: "Order",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CustomerIp",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderDiscount",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderGuid",
                table: "Order",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "OrderShippingExclTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderShippingInclTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderShippingTaxRate",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OrderStatusId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderSubTotalDiscountExclTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderSubTotalDiscountInclTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderSubtotalExclTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderSubtotalInclTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDateUtc",
                table: "Order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentMethodAdditionalFeeExclTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentMethodAdditionalFeeInclTax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentMethodAdditionalFeeTaxRate",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodSystemName",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatusId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundedAmount",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingMethod",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShippingStatusId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaxRates",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OrderNote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderNote_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackingUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShippedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipment_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentItem_Shipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_BillingAddressId",
                table: "Order",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShippingAddressId",
                table: "Order",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderNote_OrderId",
                table: "OrderNote",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_OrderId",
                table: "Shipment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItem_ShipmentId",
                table: "ShipmentItem",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Address_BillingAddressId",
                table: "Order",
                column: "BillingAddressId",
                principalTable: "Address",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Address_ShippingAddressId",
                table: "Order",
                column: "ShippingAddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Address_BillingAddressId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Address_ShippingAddressId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "OrderNote");

            migrationBuilder.DropTable(
                name: "ShipmentItem");

            migrationBuilder.DropTable(
                name: "Shipment");

            migrationBuilder.DropIndex(
                name: "IX_Order_BillingAddressId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ShippingAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CustomerIp",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderDiscount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderGuid",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderShippingExclTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderShippingInclTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderShippingTaxRate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderSubTotalDiscountExclTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderSubTotalDiscountInclTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderSubtotalExclTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderSubtotalInclTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaidDateUtc",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentMethodAdditionalFeeExclTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentMethodAdditionalFeeInclTax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentMethodAdditionalFeeTaxRate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentMethodSystemName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentStatusId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RefundedAmount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingMethod",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingStatusId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TaxRates",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "VatNumber",
                table: "Order",
                newName: "OrderNo");

            migrationBuilder.RenameColumn(
                name: "OrderTotal",
                table: "Order",
                newName: "TotalAmount");
        }
    }
}
