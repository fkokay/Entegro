using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductMediaFileEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductMediaFile_MediaFile_MediaFileId",
                table: "ProductMediaFile");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMediaFile_Product_ProductId",
                table: "ProductMediaFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductMediaFile",
                table: "ProductMediaFile");

            migrationBuilder.RenameTable(
                name: "ProductMediaFile",
                newName: "Product_MediaFile_Mapping");

            migrationBuilder.RenameIndex(
                name: "IX_ProductMediaFile_ProductId",
                table: "Product_MediaFile_Mapping",
                newName: "IX_Product_MediaFile_Mapping_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductMediaFile_MediaFileId",
                table: "Product_MediaFile_Mapping",
                newName: "IX_Product_MediaFile_Mapping_MediaFileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product_MediaFile_Mapping",
                table: "Product_MediaFile_Mapping",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_MediaFile_Mapping_MediaFile_MediaFileId",
                table: "Product_MediaFile_Mapping",
                column: "MediaFileId",
                principalTable: "MediaFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_MediaFile_Mapping_Product_ProductId",
                table: "Product_MediaFile_Mapping",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_MediaFile_Mapping_MediaFile_MediaFileId",
                table: "Product_MediaFile_Mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_MediaFile_Mapping_Product_ProductId",
                table: "Product_MediaFile_Mapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product_MediaFile_Mapping",
                table: "Product_MediaFile_Mapping");

            migrationBuilder.RenameTable(
                name: "Product_MediaFile_Mapping",
                newName: "ProductMediaFile");

            migrationBuilder.RenameIndex(
                name: "IX_Product_MediaFile_Mapping_ProductId",
                table: "ProductMediaFile",
                newName: "IX_ProductMediaFile_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_MediaFile_Mapping_MediaFileId",
                table: "ProductMediaFile",
                newName: "IX_ProductMediaFile_MediaFileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductMediaFile",
                table: "ProductMediaFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMediaFile_MediaFile_MediaFileId",
                table: "ProductMediaFile",
                column: "MediaFileId",
                principalTable: "MediaFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMediaFile_Product_ProductId",
                table: "ProductMediaFile",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
