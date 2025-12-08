using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class product_table_source : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportProfiles",
                table: "ImportProfiles");

            migrationBuilder.RenameTable(
                name: "ImportProfiles",
                newName: "ImportProfile");

            migrationBuilder.AddColumn<int>(
                name: "SourceImportProfileId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceIntegrationSystemId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceType",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportProfile",
                table: "ImportProfile",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportProfile",
                table: "ImportProfile");

            migrationBuilder.DropColumn(
                name: "SourceImportProfileId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SourceIntegrationSystemId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "ImportProfile",
                newName: "ImportProfiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportProfiles",
                table: "ImportProfiles",
                column: "Id");
        }
    }
}
