using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWT.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOfUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExp",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "Users",
                newName: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Login");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExp",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }
    }
}
