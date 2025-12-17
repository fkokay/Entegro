using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_return_request : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequest_Customer_CustomerId",
                table: "ReturnRequest");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequest_CustomerId",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "ReasonForReturn",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "RefundToWallet",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "RequestedAction",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "RequestedActionUpdatedOnUtc",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "ReturnRequestStatusId",
                table: "ReturnRequest");

            migrationBuilder.RenameColumn(
                name: "StaffNotes",
                table: "ReturnRequest",
                newName: "OrderNumber");

            migrationBuilder.RenameColumn(
                name: "CustomerComments",
                table: "ReturnRequest",
                newName: "CustomerLastName");

            migrationBuilder.AddColumn<string>(
                name: "CargoProviderName",
                table: "ReturnRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CargoTrackingLink",
                table: "ReturnRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CargoTrackingNumber",
                table: "ReturnRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimDate",
                table: "ReturnRequest",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CustomerFirstName",
                table: "ReturnRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IntegrationSystemId",
                table: "ReturnRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "ReturnRequest",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "ReturnRequest",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "OrderOutboundPackageId",
                table: "ReturnRequest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "OrderShipmentPackageId",
                table: "ReturnRequest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ReturnRequestItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnRequestId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MerchantSku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatBaseAmount = table.Column<int>(type: "int", nullable: false),
                    VatRate = table.Column<int>(type: "int", nullable: false),
                    SalesCampaignId = table.Column<int>(type: "int", nullable: false),
                    ProductCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerClaimReasonName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerClaimReasonCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlatformClaimReasonName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlatformClaimReasonCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlatformName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutoApproveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resolved = table.Column<bool>(type: "bit", nullable: false),
                    AcceptedBySeller = table.Column<bool>(type: "bit", nullable: true),
                    ReturnRequestStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnRequestItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnRequestItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReturnRequestItem_ReturnRequest_ReturnRequestId",
                        column: x => x.ReturnRequestId,
                        principalTable: "ReturnRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequest_IntegrationSystemId",
                table: "ReturnRequest",
                column: "IntegrationSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequestItem_ProductId",
                table: "ReturnRequestItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequestItem_ReturnRequestId",
                table: "ReturnRequestItem",
                column: "ReturnRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequest_IntegrationSystem_IntegrationSystemId",
                table: "ReturnRequest",
                column: "IntegrationSystemId",
                principalTable: "IntegrationSystem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequest_IntegrationSystem_IntegrationSystemId",
                table: "ReturnRequest");

            migrationBuilder.DropTable(
                name: "ReturnRequestItem");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequest_IntegrationSystemId",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "CargoProviderName",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "CargoTrackingLink",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "CargoTrackingNumber",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "ClaimDate",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "CustomerFirstName",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "IntegrationSystemId",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "OrderOutboundPackageId",
                table: "ReturnRequest");

            migrationBuilder.DropColumn(
                name: "OrderShipmentPackageId",
                table: "ReturnRequest");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                table: "ReturnRequest",
                newName: "StaffNotes");

            migrationBuilder.RenameColumn(
                name: "CustomerLastName",
                table: "ReturnRequest",
                newName: "CustomerComments");

            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "ReturnRequest",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ReturnRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "ReturnRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ReturnRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForReturn",
                table: "ReturnRequest",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "RefundToWallet",
                table: "ReturnRequest",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedAction",
                table: "ReturnRequest",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedActionUpdatedOnUtc",
                table: "ReturnRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnRequestStatusId",
                table: "ReturnRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequest_CustomerId",
                table: "ReturnRequest",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequest_Customer_CustomerId",
                table: "ReturnRequest",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
