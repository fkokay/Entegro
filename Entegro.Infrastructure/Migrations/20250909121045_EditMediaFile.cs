using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditMediaFile : Migration
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
