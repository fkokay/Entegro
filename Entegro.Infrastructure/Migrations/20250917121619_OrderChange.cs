using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyRate",
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
                name: "OrderSourceId",
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
                name: "PaymentMethodSystemName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TaxRates",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "VatNumber",
                table: "Order",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodAdditionalFeeTaxRate",
                table: "Order",
                newName: "PaymentFee");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodAdditionalFeeInclTax",
                table: "Order",
                newName: "OrderSubTotal");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodAdditionalFeeExclTax",
                table: "Order",
                newName: "OrderShipping");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "Order",
                newName: "OrderDateUtc");

            migrationBuilder.RenameColumn(
                name: "CustomerIp",
                table: "Order",
                newName: "IntegrationOrderNumber");

            migrationBuilder.AddColumn<string>(
                name: "AttributesXml",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ItemWeight",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductCost",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributesXml",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ItemWeight",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ProductCost",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "Order",
                newName: "VatNumber");

            migrationBuilder.RenameColumn(
                name: "PaymentFee",
                table: "Order",
                newName: "PaymentMethodAdditionalFeeTaxRate");

            migrationBuilder.RenameColumn(
                name: "OrderSubTotal",
                table: "Order",
                newName: "PaymentMethodAdditionalFeeInclTax");

            migrationBuilder.RenameColumn(
                name: "OrderShipping",
                table: "Order",
                newName: "PaymentMethodAdditionalFeeExclTax");

            migrationBuilder.RenameColumn(
                name: "OrderDateUtc",
                table: "Order",
                newName: "OrderDate");

            migrationBuilder.RenameColumn(
                name: "IntegrationOrderNumber",
                table: "Order",
                newName: "CustomerIp");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyRate",
                table: "Order",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

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
                name: "OrderSourceId",
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

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodSystemName",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaxRates",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
