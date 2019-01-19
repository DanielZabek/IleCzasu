using Microsoft.EntityFrameworkCore.Migrations;

namespace IleCzasu.Infrastructure.Migrations
{
    public partial class AddedIconClassToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconClass",
                table: "Categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconClass",
                table: "Categories");
        }
    }
}
