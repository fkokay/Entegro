using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class edit_address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "TownId",
                table: "Address");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "Address");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TownId",
                table: "Address",
                type: "int",
                nullable: true);
        }
    }
}
