using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductVariantAttributeValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductVariantAttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantAttributeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariantAttributeValue_Product_ProductAttribute_Mapping_ProductVariantAttributeId",
                        column: x => x.ProductVariantAttributeId,
                        principalTable: "Product_ProductAttribute_Mapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttributeValue_ProductVariantAttributeId",
                table: "ProductVariantAttributeValue",
                column: "ProductVariantAttributeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVariantAttributeValue");
        }
    }
}
