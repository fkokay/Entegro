using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditGenel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Product",
                newName: "UpdatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Product",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Customer",
                newName: "UpdatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Customer",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Category",
                newName: "UpdatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Category",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Brand",
                newName: "UpdatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Brand",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Address",
                newName: "UpdatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Address",
                newName: "CreatedOnUtc");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Category",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Category");

            migrationBuilder.RenameColumn(
                name: "UpdatedOnUtc",
                table: "Product",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Product",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "UpdatedOnUtc",
                table: "Customer",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Customer",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "UpdatedOnUtc",
                table: "Category",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Category",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "UpdatedOnUtc",
                table: "Brand",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Brand",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "UpdatedOnUtc",
                table: "Address",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Address",
                newName: "CreatedOn");
        }
    }
}
