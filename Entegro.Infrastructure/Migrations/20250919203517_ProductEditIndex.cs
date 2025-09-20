using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductEditIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpecialPrice",
                table: "Product",
                newName: "SalePrice");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ManufacturerPartNumber",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gtin",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Product",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "IntegrationSku",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntegrationSystemId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTransient",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_Code",
                table: "Product",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Deleted",
                table: "Product",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Gtin",
                table: "Product",
                column: "Gtin");

            migrationBuilder.CreateIndex(
                name: "IX_Product_IntegrationSystemId",
                table: "Product",
                column: "IntegrationSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_IsTransient",
                table: "Product",
                column: "IsTransient");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ManufacturerPartNumber",
                table: "Product",
                column: "ManufacturerPartNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Name",
                table: "Product",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Published",
                table: "Product",
                column: "Published");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_IntegrationSystem_IntegrationSystemId",
                table: "Product",
                column: "IntegrationSystemId",
                principalTable: "IntegrationSystem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_IntegrationSystem_IntegrationSystemId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Code",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Deleted",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Gtin",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_IntegrationSystemId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_IsTransient",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ManufacturerPartNumber",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Name",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Published",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IntegrationSku",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IntegrationSystemId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsTransient",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "SalePrice",
                table: "Product",
                newName: "SpecialPrice");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ManufacturerPartNumber",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gtin",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
