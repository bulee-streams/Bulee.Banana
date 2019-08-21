using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddedSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "EmailConfirmationToken", "EmailConfirmed", "Password", "Salt", "Username" },
                values: new object[] { new Guid("0d83e045-49ce-4434-bed5-267f0dbec7f7"), "user@email.com", new Guid("a6a46a35-5165-4ab5-9e19-12764cfc2144"), false, null, null, "user" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("0d83e045-49ce-4434-bed5-267f0dbec7f7"));
        }
    }
}
