using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addrelatedproductproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RelatedProduct_ProductId1_ProductId2",
                table: "RelatedProduct",
                columns: new[] { "ProductId1", "ProductId2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedProduct_ProductId2",
                table: "RelatedProduct",
                column: "ProductId2");

            migrationBuilder.AddForeignKey(
                name: "FK_RelatedProduct_Product_ProductId1",
                table: "RelatedProduct",
                column: "ProductId1",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RelatedProduct_Product_ProductId2",
                table: "RelatedProduct",
                column: "ProductId2",
                principalTable: "Product",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelatedProduct_Product_ProductId1",
                table: "RelatedProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_RelatedProduct_Product_ProductId2",
                table: "RelatedProduct");

            migrationBuilder.DropIndex(
                name: "IX_RelatedProduct_ProductId1_ProductId2",
                table: "RelatedProduct");

            migrationBuilder.DropIndex(
                name: "IX_RelatedProduct_ProductId2",
                table: "RelatedProduct");
        }
    }
}
