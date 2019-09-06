using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class BananaMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("3572bc96-2536-4f22-b69a-ed89ae4f7d66"));

            migrationBuilder.AddColumn<Guid>(
                name: "PassworResetToken",
                table: "User",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "EmailConfirmationToken", "EmailConfirmed", "PassworResetToken", "Password", "Salt", "Username" },
                values: new object[] { new Guid("8cec4b25-4563-463f-a690-498f60c743e9"), "user@email.com", new Guid("a6a46a35-5165-4ab5-9e19-12764cfc2144"), false, new Guid("00000000-0000-0000-0000-000000000000"), "QWZjuWqZvjVItYJyHQc69LHCeYguawJdYgvELNszsdI=", new byte[] { 23, 69, 221, 70, 219, 2, 73, 116, 196, 47, 121, 144, 89, 11, 131, 74 }, "user" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("8cec4b25-4563-463f-a690-498f60c743e9"));

            migrationBuilder.DropColumn(
                name: "PassworResetToken",
                table: "User");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "EmailConfirmationToken", "EmailConfirmed", "Password", "Salt", "Username" },
                values: new object[] { new Guid("3572bc96-2536-4f22-b69a-ed89ae4f7d66"), "user@email.com", new Guid("a6a46a35-5165-4ab5-9e19-12764cfc2144"), false, "Z+rTKRldeJu7FKNCXwZF41l8I0plylCl2qwd0nxPGNg=", new byte[] { 63, 145, 25, 252, 219, 98, 134, 192, 254, 198, 168, 251, 41, 32, 117, 121 }, "user" });
        }
    }
}
