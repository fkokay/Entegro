using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editmediafile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFile_MediaFolder_FolderId",
                table: "MediaFile");

            migrationBuilder.DropIndex(
                name: "IX_MediaFile_FolderId",
                table: "MediaFile");

            migrationBuilder.AddColumn<int>(
                name: "MediaFolderId",
                table: "MediaFile",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductIntegration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IntegrationSystemId = table.Column<int>(type: "int", nullable: false),
                    LastSyncDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductIntegration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductIntegration_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFile_MediaFolderId",
                table: "MediaFile",
                column: "MediaFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductIntegration_ProductId",
                table: "ProductIntegration",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFile_MediaFolder_MediaFolderId",
                table: "MediaFile",
                column: "MediaFolderId",
                principalTable: "MediaFolder",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFile_MediaFolder_MediaFolderId",
                table: "MediaFile");

            migrationBuilder.DropTable(
                name: "ProductIntegration");

            migrationBuilder.DropIndex(
                name: "IX_MediaFile_MediaFolderId",
                table: "MediaFile");

            migrationBuilder.DropColumn(
                name: "MediaFolderId",
                table: "MediaFile");

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
        }
    }
}
