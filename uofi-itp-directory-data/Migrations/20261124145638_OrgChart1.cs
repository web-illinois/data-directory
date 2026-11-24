using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uofi_itp_directory_data.Migrations
{
    /// <inheritdoc />
    public partial class OrgChart1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerJobProfileId1",
                table: "Offices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerJobProfileId2",
                table: "Offices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerOfficeId",
                table: "Offices",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerJobProfileId1",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "ManagerJobProfileId2",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "ManagerOfficeId",
                table: "Offices");
        }
    }
}
