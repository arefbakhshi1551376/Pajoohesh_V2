using Microsoft.EntityFrameworkCore.Migrations;

namespace Pajoohesh_V2.Data.Migrations
{
    public partial class addImageUrlToSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Subjects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Subjects");
        }
    }
}
