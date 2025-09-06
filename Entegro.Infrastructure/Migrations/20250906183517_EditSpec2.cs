using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditSpec2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecificationAttribute_Product_ProductId",
                table: "ProductSpecificationAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSpecificationAttribute_SpecificationAttributeOption_SpecificationAttributeOptionId",
                table: "ProductSpecificationAttribute");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSpecificationAttribute",
                table: "ProductSpecificationAttribute");

            migrationBuilder.RenameTable(
                name: "ProductSpecificationAttribute",
                newName: "Product_SpecificationAttribute_Mapping");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSpecificationAttribute_SpecificationAttributeOptionId",
                table: "Product_SpecificationAttribute_Mapping",
                newName: "IX_Product_SpecificationAttribute_Mapping_SpecificationAttributeOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSpecificationAttribute_ProductId",
                table: "Product_SpecificationAttribute_Mapping",
                newName: "IX_Product_SpecificationAttribute_Mapping_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product_SpecificationAttribute_Mapping",
                table: "Product_SpecificationAttribute_Mapping",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_SpecificationAttribute_Mapping_Product_ProductId",
                table: "Product_SpecificationAttribute_Mapping",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_SpecificationAttribute_Mapping_SpecificationAttributeOption_SpecificationAttributeOptionId",
                table: "Product_SpecificationAttribute_Mapping",
                column: "SpecificationAttributeOptionId",
                principalTable: "SpecificationAttributeOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_SpecificationAttribute_Mapping_Product_ProductId",
                table: "Product_SpecificationAttribute_Mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_SpecificationAttribute_Mapping_SpecificationAttributeOption_SpecificationAttributeOptionId",
                table: "Product_SpecificationAttribute_Mapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product_SpecificationAttribute_Mapping",
                table: "Product_SpecificationAttribute_Mapping");

            migrationBuilder.RenameTable(
                name: "Product_SpecificationAttribute_Mapping",
                newName: "ProductSpecificationAttribute");

            migrationBuilder.RenameIndex(
                name: "IX_Product_SpecificationAttribute_Mapping_SpecificationAttributeOptionId",
                table: "ProductSpecificationAttribute",
                newName: "IX_ProductSpecificationAttribute_SpecificationAttributeOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_SpecificationAttribute_Mapping_ProductId",
                table: "ProductSpecificationAttribute",
                newName: "IX_ProductSpecificationAttribute_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSpecificationAttribute",
                table: "ProductSpecificationAttribute",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecificationAttribute_Product_ProductId",
                table: "ProductSpecificationAttribute",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSpecificationAttribute_SpecificationAttributeOption_SpecificationAttributeOptionId",
                table: "ProductSpecificationAttribute",
                column: "SpecificationAttributeOptionId",
                principalTable: "SpecificationAttributeOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
