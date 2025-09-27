using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivityEdit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_ActivityLogTypes_ActivityLogTypeId",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_User_UserId",
                table: "ActivityLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLogTypes",
                table: "ActivityLogTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLogs",
                table: "ActivityLogs");

            migrationBuilder.RenameTable(
                name: "ActivityLogTypes",
                newName: "ActivityLogType");

            migrationBuilder.RenameTable(
                name: "ActivityLogs",
                newName: "ActivityLog");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLog",
                newName: "IX_ActivityLog_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_ActivityLogTypeId",
                table: "ActivityLog",
                newName: "IX_ActivityLog_ActivityLogTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLogType",
                table: "ActivityLogType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLog",
                table: "ActivityLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_ActivityLogType_ActivityLogTypeId",
                table: "ActivityLog",
                column: "ActivityLogTypeId",
                principalTable: "ActivityLogType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_User_UserId",
                table: "ActivityLog",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_ActivityLogType_ActivityLogTypeId",
                table: "ActivityLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_User_UserId",
                table: "ActivityLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLogType",
                table: "ActivityLogType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLog",
                table: "ActivityLog");

            migrationBuilder.RenameTable(
                name: "ActivityLogType",
                newName: "ActivityLogTypes");

            migrationBuilder.RenameTable(
                name: "ActivityLog",
                newName: "ActivityLogs");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLog_UserId",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLog_ActivityLogTypeId",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_ActivityLogTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLogTypes",
                table: "ActivityLogTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLogs",
                table: "ActivityLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_ActivityLogTypes_ActivityLogTypeId",
                table: "ActivityLogs",
                column: "ActivityLogTypeId",
                principalTable: "ActivityLogTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_User_UserId",
                table: "ActivityLogs",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
