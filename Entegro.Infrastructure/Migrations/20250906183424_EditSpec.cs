using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditSpec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSpecificationAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationAttributeOptionId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecificationAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSpecificationAttribute_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSpecificationAttribute_SpecificationAttributeOption_SpecificationAttributeOptionId",
                        column: x => x.SpecificationAttributeOptionId,
                        principalTable: "SpecificationAttributeOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationAttribute_ProductId",
                table: "ProductSpecificationAttribute",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationAttribute_SpecificationAttributeOptionId",
                table: "ProductSpecificationAttribute",
                column: "SpecificationAttributeOptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSpecificationAttribute");
        }
    }
}
