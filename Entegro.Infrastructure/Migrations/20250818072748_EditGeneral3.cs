using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditGeneral3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MediaFolder_ParentId",
                table: "MediaFolder",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFolder_MediaFolder_ParentId",
                table: "MediaFolder",
                column: "ParentId",
                principalTable: "MediaFolder",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFolder_MediaFolder_ParentId",
                table: "MediaFolder");

            migrationBuilder.DropIndex(
                name: "IX_MediaFolder_ParentId",
                table: "MediaFolder");
        }
    }
}
