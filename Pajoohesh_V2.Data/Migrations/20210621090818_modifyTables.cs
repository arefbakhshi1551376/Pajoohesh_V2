using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pajoohesh_V2.Data.Migrations
{
    public partial class modifyTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReplySubject",
                table: "ContactUss",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplyText",
                table: "ContactUss",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AboutUs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    CreatorId = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    LastModifierId = table.Column<string>(nullable: true),
                    LastModifyDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AboutUs_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AboutUs_AspNetUsers_LastModifierId",
                        column: x => x.LastModifierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AboutUs_CreatorId",
                table: "AboutUs",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AboutUs_LastModifierId",
                table: "AboutUs",
                column: "LastModifierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUs");

            migrationBuilder.DropColumn(
                name: "ReplySubject",
                table: "ContactUss");

            migrationBuilder.DropColumn(
                name: "ReplyText",
                table: "ContactUss");
        }
    }
}
