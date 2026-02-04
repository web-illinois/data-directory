using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uofi_itp_directory_data.Migrations
{
    /// <inheritdoc />
    public partial class APIKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiSecretCurrent",
                table: "AreaSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApiSecretLastChanged",
                table: "AreaSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiSecretPrevious",
                table: "AreaSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -2,
                column: "LastUpdated",
                value: new DateTime(2026, 2, 4, 9, 10, 30, 839, DateTimeKind.Local).AddTicks(6007));

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2026, 2, 4, 9, 10, 30, 839, DateTimeKind.Local).AddTicks(5888));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiSecretCurrent",
                table: "AreaSettings");

            migrationBuilder.DropColumn(
                name: "ApiSecretLastChanged",
                table: "AreaSettings");

            migrationBuilder.DropColumn(
                name: "ApiSecretPrevious",
                table: "AreaSettings");

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
        }
    }
}
