using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarinLend.Data.Migrations
{
    /// <inheritdoc />
    public partial class change_AyandehSignRequestSignatureLog_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "AyandehSignRequestSignatureLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "AyandehSignRequestSignatureLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Curl",
                table: "AyandehSignRequestSignatureLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackId",
                table: "AyandehSignRequestSignatureLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Curl",
                table: "AyandehSignRequestSignatureLogs");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "AyandehSignRequestSignatureLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "AyandehSignRequestSignatureLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "AyandehSignRequestSignatureLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
