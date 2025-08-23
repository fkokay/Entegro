using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Product_MainPictureId",
                table: "Product",
                column: "MainPictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_MediaFile_MainPictureId",
                table: "Product",
                column: "MainPictureId",
                principalTable: "MediaFile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_MediaFile_MainPictureId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_MainPictureId",
                table: "Product");
        }
    }
}
