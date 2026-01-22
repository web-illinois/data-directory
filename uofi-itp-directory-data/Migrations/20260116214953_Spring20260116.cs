using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uofi_itp_directory_data.Migrations
{
    /// <inheritdoc />
    public partial class Spring20260116 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlternateContact",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AlternateContactEmail",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AlternateContactPhone",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "UseAlternateContactAsPrimary",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InstructionsEmployeeCourses",
                table: "AreaSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EmployeeCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    InternalOrder = table.Column<int>(type: "int", nullable: false),
                    Rubric = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeCourses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -2,
                column: "LastUpdated",
                value: new DateTime(2026, 1, 16, 15, 49, 51, 6, DateTimeKind.Local).AddTicks(7331));

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2026, 1, 16, 15, 49, 51, 6, DateTimeKind.Local).AddTicks(7207));

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCourses_EmployeeId",
                table: "EmployeeCourses",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeCourses");

            migrationBuilder.DropColumn(
                name: "AlternateContact",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AlternateContactEmail",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AlternateContactPhone",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UseAlternateContactAsPrimary",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "InstructionsEmployeeCourses",
                table: "AreaSettings");

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -2,
                column: "LastUpdated",
                value: new DateTime(2025, 7, 11, 12, 28, 35, 16, DateTimeKind.Local).AddTicks(9185));

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2025, 7, 11, 12, 28, 35, 16, DateTimeKind.Local).AddTicks(8709));
        }
    }
}
