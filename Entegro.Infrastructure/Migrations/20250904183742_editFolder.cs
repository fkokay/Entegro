using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFile_MediaFolder_FolderId",
                table: "MediaFile");

            migrationBuilder.RenameColumn(
                name: "FolderId",
                table: "MediaFile",
                newName: "MediaFolderId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaFile_FolderId",
                table: "MediaFile",
                newName: "IX_MediaFile_MediaFolderId");

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

            migrationBuilder.RenameColumn(
                name: "MediaFolderId",
                table: "MediaFile",
                newName: "FolderId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaFile_MediaFolderId",
                table: "MediaFile",
                newName: "IX_MediaFile_FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFile_MediaFolder_FolderId",
                table: "MediaFile",
                column: "FolderId",
                principalTable: "MediaFolder",
                principalColumn: "Id");
        }
    }
}
