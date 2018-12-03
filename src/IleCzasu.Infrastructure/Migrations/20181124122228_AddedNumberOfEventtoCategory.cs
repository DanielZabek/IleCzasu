using Microsoft.EntityFrameworkCore.Migrations;

namespace IleCzasu.Infrastructure.Migrations
{
    public partial class AddedNumberOfEventtoCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfEvents",
                table: "Categories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId1",
                table: "Events",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UserId1",
                table: "Events",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UserId1",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_UserId1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NumberOfEvents",
                table: "Categories");
        }
    }
}
