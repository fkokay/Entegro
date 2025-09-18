using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_prop_importprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MediaFileId",
                table: "ImportProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "ApplyPriceAdjustment",
                table: "ImportProfiles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OptionalExtraAmount",
                table: "ImportProfiles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAdjustmentAmount",
                table: "ImportProfiles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceAdjustmentType",
                table: "ImportProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyPriceAdjustment",
                table: "ImportProfiles");

            migrationBuilder.DropColumn(
                name: "OptionalExtraAmount",
                table: "ImportProfiles");

            migrationBuilder.DropColumn(
                name: "PriceAdjustmentAmount",
                table: "ImportProfiles");

            migrationBuilder.DropColumn(
                name: "PriceAdjustmentType",
                table: "ImportProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "MediaFileId",
                table: "ImportProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
