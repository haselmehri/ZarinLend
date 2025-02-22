using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarinLend.Data.Migrations
{
    /// <inheritdoc />
    public partial class change_FacilityRequest_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AyandehSignSigningTokenForAdminBank",
                table: "RequestFacilities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SigningMethod",
                table: "RequestFacilities",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AyandehSignSigningTokenForAdminBank",
                table: "RequestFacilities");

            migrationBuilder.DropColumn(
                name: "SigningMethod",
                table: "RequestFacilities");
        }
    }
}
