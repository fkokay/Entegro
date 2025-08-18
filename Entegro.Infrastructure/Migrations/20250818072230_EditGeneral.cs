using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditGeneral : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "ProductImageMapping");

            migrationBuilder.AddColumn<int>(
                name: "MediaFileId",
                table: "ProductImageMapping",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TreePath",
                table: "Category",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "MediaFileId",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MediaFileId",
                table: "Brand",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImageMapping_MediaFileId",
                table: "ProductImageMapping",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeMapping_ProductAttributeId",
                table: "ProductAttributeMapping",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeMapping_ProductId",
                table: "ProductAttributeMapping",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFile_FolderId",
                table: "MediaFile",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_MediaFileId",
                table: "Category",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Brand_MediaFileId",
                table: "Brand",
                column: "MediaFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_MediaFile_MediaFileId",
                table: "Brand",
                column: "MediaFileId",
                principalTable: "MediaFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_MediaFile_MediaFileId",
                table: "Category",
                column: "MediaFileId",
                principalTable: "MediaFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFile_MediaFolder_FolderId",
                table: "MediaFile",
                column: "FolderId",
                principalTable: "MediaFolder",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeMapping_ProductAttribute_ProductAttributeId",
                table: "ProductAttributeMapping",
                column: "ProductAttributeId",
                principalTable: "ProductAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeMapping_Product_ProductId",
                table: "ProductAttributeMapping",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImageMapping_MediaFile_MediaFileId",
                table: "ProductImageMapping",
                column: "MediaFileId",
                principalTable: "MediaFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brand_MediaFile_MediaFileId",
                table: "Brand");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_MediaFile_MediaFileId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFile_MediaFolder_FolderId",
                table: "MediaFile");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeMapping_ProductAttribute_ProductAttributeId",
                table: "ProductAttributeMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeMapping_Product_ProductId",
                table: "ProductAttributeMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImageMapping_MediaFile_MediaFileId",
                table: "ProductImageMapping");

            migrationBuilder.DropIndex(
                name: "IX_ProductImageMapping_MediaFileId",
                table: "ProductImageMapping");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeMapping_ProductAttributeId",
                table: "ProductAttributeMapping");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeMapping_ProductId",
                table: "ProductAttributeMapping");

            migrationBuilder.DropIndex(
                name: "IX_MediaFile_FolderId",
                table: "MediaFile");

            migrationBuilder.DropIndex(
                name: "IX_Category_MediaFileId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Brand_MediaFileId",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "MediaFileId",
                table: "ProductImageMapping");

            migrationBuilder.DropColumn(
                name: "MediaFileId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "MediaFileId",
                table: "Brand");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ProductImageMapping",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TreePath",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);
        }
    }
}
