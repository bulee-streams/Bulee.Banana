using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class BananaMigration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmationToken = table.Column<Guid>(nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Salt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "EmailConfirmationToken", "EmailConfirmed", "Password", "Salt", "Username" },
                values: new object[] { new Guid("3572bc96-2536-4f22-b69a-ed89ae4f7d66"), "user@email.com", new Guid("a6a46a35-5165-4ab5-9e19-12764cfc2144"), false, "Z+rTKRldeJu7FKNCXwZF41l8I0plylCl2qwd0nxPGNg=", new byte[] { 63, 145, 25, 252, 219, 98, 134, 192, 254, 198, 168, 251, 41, 32, 117, 121 }, "user" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
