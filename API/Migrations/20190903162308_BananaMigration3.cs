using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class BananaMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("8cec4b25-4563-463f-a690-498f60c743e9"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "EmailConfirmationToken", "EmailConfirmed", "PassworResetToken", "Password", "Salt", "Username" },
                values: new object[] { new Guid("72112158-50f3-43a6-80c3-8bc3ba33cf9d"), "user@email.com", new Guid("a6a46a35-5165-4ab5-9e19-12764cfc2144"), false, new Guid("214065dd-36b2-4e5e-a67b-37aab766bafa"), "Kg8UhoPIigwyNIUDxbSC+nkyX+TQ34kYksFZkWuAw/4=", new byte[] { 54, 54, 218, 252, 128, 49, 75, 36, 109, 26, 136, 21, 86, 217, 9, 189 }, "user" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("72112158-50f3-43a6-80c3-8bc3ba33cf9d"));

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "EmailConfirmationToken", "EmailConfirmed", "PassworResetToken", "Password", "Salt", "Username" },
                values: new object[] { new Guid("8cec4b25-4563-463f-a690-498f60c743e9"), "user@email.com", new Guid("a6a46a35-5165-4ab5-9e19-12764cfc2144"), false, new Guid("00000000-0000-0000-0000-000000000000"), "QWZjuWqZvjVItYJyHQc69LHCeYguawJdYgvELNszsdI=", new byte[] { 23, 69, 221, 70, 219, 2, 73, 116, 196, 47, 121, 144, 89, 11, 131, 74 }, "user" });
        }
    }
}
