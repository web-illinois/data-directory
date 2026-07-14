using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace uofi_itp_directory_data.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SecurityEntries",
                columns: new[] { "Id", "AreaId", "CanEditAllPeopleInUnit", "Email", "IsActive", "IsFullAdmin", "IsPublic", "LastUpdated", "ListedNameFirst", "ListedNameLast", "OfficeId" },
                values: new object[,]
                {
                    { -2, null, true, "rbwatson@illinois.edu", true, true, false, new DateTime(2026, 2, 4, 9, 10, 30, 839, DateTimeKind.Local).AddTicks(6007), "Rob", "Watson", null },
                    { -1, null, true, "jonker@illinois.edu", true, true, false, new DateTime(2026, 2, 4, 9, 10, 30, 839, DateTimeKind.Local).AddTicks(5888), "Bryan", "Jonker", null }
                });
        }
    }
}
