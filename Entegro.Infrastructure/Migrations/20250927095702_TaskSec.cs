using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entegro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TaskSec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    StopOnError = table.Column<bool>(type: "bit", nullable: false),
                    NextRunUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    RunPerMachine = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTaskHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleTaskId = table.Column<int>(type: "int", nullable: false),
                    IsRunning = table.Column<bool>(type: "bit", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    StartedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SucceededOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgressPercent = table.Column<int>(type: "int", nullable: true),
                    ProgressMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTaskHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleTaskHistory_ScheduleTask_ScheduleTaskId",
                        column: x => x.ScheduleTaskId,
                        principalTable: "ScheduleTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NextRun_Enabled",
                table: "ScheduleTask",
                columns: new[] { "NextRunUtc", "Enabled" });

            migrationBuilder.CreateIndex(
                name: "IX_Type",
                table: "ScheduleTask",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_MachineName_IsRunning",
                table: "ScheduleTaskHistory",
                columns: new[] { "MachineName", "IsRunning" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTaskHistory_ScheduleTaskId",
                table: "ScheduleTaskHistory",
                column: "ScheduleTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Started_Finished",
                table: "ScheduleTaskHistory",
                columns: new[] { "StartedOnUtc", "FinishedOnUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleTaskHistory");

            migrationBuilder.DropTable(
                name: "ScheduleTask");
        }
    }
}
