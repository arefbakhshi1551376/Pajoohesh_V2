using Microsoft.EntityFrameworkCore.Migrations;

namespace Pajoohesh_V2.Data.Migrations
{
    public partial class modifyLikeUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Films_FilmId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FilmId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FilmId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "FilmLikeUser",
                columns: table => new
                {
                    FilmId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmLikeUser", x => new { x.FilmId, x.UserId });
                    table.ForeignKey(
                        name: "FK_FilmLikeUser_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmLikeUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmLikeUser_UserId",
                table: "FilmLikeUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmLikeUser");

            migrationBuilder.AddColumn<int>(
                name: "FilmId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FilmId",
                table: "AspNetUsers",
                column: "FilmId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Films_FilmId",
                table: "AspNetUsers",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
