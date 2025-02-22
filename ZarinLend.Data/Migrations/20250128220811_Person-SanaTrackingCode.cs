using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarinLend.Data.Migrations
{
    /// <inheritdoc />
    public partial class PersonSanaTrackingCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SanaTrackingId",
                table: "People",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SanaTrackingId",
                table: "People");
        }
    }
}
