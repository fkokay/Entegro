using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class relatedproduct_crosssellproduct_deletebehavior_change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrossSellProduct_Product_ProductId1",
                table: "CrossSellProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_CrossSellProduct_Product_ProductId2",
                table: "CrossSellProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_CrossSellProduct_Product_ProductId1",
                table: "CrossSellProduct",
                column: "ProductId1",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CrossSellProduct_Product_ProductId2",
                table: "CrossSellProduct",
                column: "ProductId2",
                principalTable: "Product",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrossSellProduct_Product_ProductId1",
                table: "CrossSellProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_CrossSellProduct_Product_ProductId2",
                table: "CrossSellProduct");

            migrationBuilder.AddForeignKey(
                name: "FK_CrossSellProduct_Product_ProductId1",
                table: "CrossSellProduct",
                column: "ProductId1",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CrossSellProduct_Product_ProductId2",
                table: "CrossSellProduct",
                column: "ProductId2",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
