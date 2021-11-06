using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pajoohesh_V2.Data.Migrations
{
    public partial class modifyContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactUss_AspNetUsers_SenderId",
                table: "ContactUss");

            migrationBuilder.DropIndex(
                name: "IX_ContactUss_SenderId",
                table: "ContactUss");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "ContactUss");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReplyDate",
                table: "ContactUss",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "SenderEmail",
                table: "ContactUss",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderFamily",
                table: "ContactUss",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "ContactUss",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderEmail",
                table: "ContactUss");

            migrationBuilder.DropColumn(
                name: "SenderFamily",
                table: "ContactUss");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "ContactUss");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReplyDate",
                table: "ContactUss",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "ContactUss",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactUss_SenderId",
                table: "ContactUss",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactUss_AspNetUsers_SenderId",
                table: "ContactUss",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
