using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarinLend.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserBankAccountClientId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBankAccounts_CardNumber",
                table: "UserBankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "UserBankAccounts",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "UserBankAccounts",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBankAccounts_CardNumber",
                table: "UserBankAccounts");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "UserBankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "UserBankAccounts",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16,
                oldNullable: true);
        }
    }
}
