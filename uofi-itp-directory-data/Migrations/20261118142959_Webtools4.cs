using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uofi_itp_directory_data.Migrations
{
    /// <inheritdoc />
    public partial class Webtools4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WebToolsAutomaticEmployeeImport",
                table: "OfficeSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WebToolsAutomaticImport",
                table: "OfficeSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "Offices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebToolsAutomaticEmployeeImport",
                table: "OfficeSettings");

            migrationBuilder.DropColumn(
                name: "WebToolsAutomaticImport",
                table: "OfficeSettings");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Offices");
        }
    }
}
