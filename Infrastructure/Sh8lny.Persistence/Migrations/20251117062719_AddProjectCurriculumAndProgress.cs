using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sh8lny.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectCurriculumAndProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectModules_ProjectId",
                table: "ProjectModules");

            migrationBuilder.CreateTable(
                name: "ApplicationModuleProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ProjectModuleId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationModuleProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationModuleProgress_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationModuleProgress_ProjectModules_ProjectModuleId",
                        column: x => x.ProjectModuleId,
                        principalTable: "ProjectModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_ProjectModules_Project_Order",
                table: "ProjectModules",
                columns: new[] { "ProjectId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationModuleProgress_ProjectModuleId",
                table: "ApplicationModuleProgress",
                column: "ProjectModuleId");

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationModuleProgress",
                table: "ApplicationModuleProgress",
                columns: new[] { "ApplicationId", "ProjectModuleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationModuleProgress");

            migrationBuilder.DropIndex(
                name: "IDX_ProjectModules_Project_Order",
                table: "ProjectModules");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectModules_ProjectId",
                table: "ProjectModules",
                column: "ProjectId");
        }
    }
}
