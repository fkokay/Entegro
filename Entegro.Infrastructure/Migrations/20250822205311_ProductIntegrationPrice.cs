using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductIntegrationPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFile_MediaFolder_MediaFolderId",
                table: "MediaFile");

            migrationBuilder.DropIndex(
                name: "IX_MediaFile_MediaFolderId",
                table: "MediaFile");

            migrationBuilder.DropColumn(
                name: "MediaFolderId",
                table: "MediaFile");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductIntegration",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ProductIntegration_IntegrationSystemId",
                table: "ProductIntegration",
                column: "IntegrationSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFile_FolderId",
                table: "MediaFile",
                column: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFile_MediaFolder_FolderId",
                table: "MediaFile",
                column: "FolderId",
                principalTable: "MediaFolder",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIntegration_IntegrationSystem_IntegrationSystemId",
                table: "ProductIntegration",
                column: "IntegrationSystemId",
                principalTable: "IntegrationSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFile_MediaFolder_FolderId",
                table: "MediaFile");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductIntegration_IntegrationSystem_IntegrationSystemId",
                table: "ProductIntegration");

            migrationBuilder.DropIndex(
                name: "IX_ProductIntegration_IntegrationSystemId",
                table: "ProductIntegration");

            migrationBuilder.DropIndex(
                name: "IX_MediaFile_FolderId",
                table: "MediaFile");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductIntegration");

            migrationBuilder.AddColumn<int>(
                name: "MediaFolderId",
                table: "MediaFile",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaFile_MediaFolderId",
                table: "MediaFile",
                column: "MediaFolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFile_MediaFolder_MediaFolderId",
                table: "MediaFile",
                column: "MediaFolderId",
                principalTable: "MediaFolder",
                principalColumn: "Id");
        }
    }
}
