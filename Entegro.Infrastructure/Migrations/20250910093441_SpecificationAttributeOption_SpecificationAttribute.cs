using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpecificationAttributeOption_SpecificationAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SpecificationAttributeOption_SpecificationAttributeId",
                table: "SpecificationAttributeOption",
                column: "SpecificationAttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificationAttributeOption_SpecificationAttribute_SpecificationAttributeId",
                table: "SpecificationAttributeOption",
                column: "SpecificationAttributeId",
                principalTable: "SpecificationAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificationAttributeOption_SpecificationAttribute_SpecificationAttributeId",
                table: "SpecificationAttributeOption");

            migrationBuilder.DropIndex(
                name: "IX_SpecificationAttributeOption_SpecificationAttributeId",
                table: "SpecificationAttributeOption");
        }
    }
}
