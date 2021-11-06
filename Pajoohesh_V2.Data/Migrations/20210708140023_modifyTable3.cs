using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pajoohesh_V2.Data.Migrations
{
    public partial class modifyTable3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHeMemberOfTheNewsletter",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsChangePasswordFinished",
                table: "ForgetPasswords",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "NewsLetters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    IsEmailVerified = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLetters", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsLetters");

            migrationBuilder.DropColumn(
                name: "IsChangePasswordFinished",
                table: "ForgetPasswords");

            migrationBuilder.AddColumn<bool>(
                name: "IsHeMemberOfTheNewsletter",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
