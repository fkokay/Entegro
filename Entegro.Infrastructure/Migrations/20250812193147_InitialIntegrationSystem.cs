using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialIntegrationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationSystem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationSystemLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntegrationSystemId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationSystemLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntegrationSystemLog_IntegrationSystem_IntegrationSystemId",
                        column: x => x.IntegrationSystemId,
                        principalTable: "IntegrationSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationSystemParameter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntegrationSystemId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationSystemParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntegrationSystemParameter_IntegrationSystem_IntegrationSystemId",
                        column: x => x.IntegrationSystemId,
                        principalTable: "IntegrationSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationSystemLog_IntegrationSystemId",
                table: "IntegrationSystemLog",
                column: "IntegrationSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationSystemParameter_IntegrationSystemId",
                table: "IntegrationSystemParameter",
                column: "IntegrationSystemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationSystemLog");

            migrationBuilder.DropTable(
                name: "IntegrationSystemParameter");

            migrationBuilder.DropTable(
                name: "IntegrationSystem");
        }
    }
}
