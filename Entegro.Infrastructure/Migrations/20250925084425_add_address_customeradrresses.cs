using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_address_customeradrresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId1",
                table: "CustomerAddressMapping",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddressMapping_AddressId1",
                table: "CustomerAddressMapping",
                column: "AddressId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddressMapping_Address_AddressId1",
                table: "CustomerAddressMapping",
                column: "AddressId1",
                principalTable: "Address",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddressMapping_Address_AddressId1",
                table: "CustomerAddressMapping");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddressMapping_AddressId1",
                table: "CustomerAddressMapping");

            migrationBuilder.DropColumn(
                name: "AddressId1",
                table: "CustomerAddressMapping");
        }
    }
}
