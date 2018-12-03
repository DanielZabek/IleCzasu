using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IleCzasu.Infrastructure.Migrations
{
    public partial class PublicEventsSplit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Events_EventId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UserId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UserId1",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Events_EventId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_TagEvents_Events_EventId",
                table: "TagEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_UserId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_UserId1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Repeatable",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "PublicEvents");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "TagEvents",
                newName: "PublicEventId");

            migrationBuilder.RenameIndex(
                name: "IX_TagEvents_EventId",
                table: "TagEvents",
                newName: "IX_TagEvents_PublicEventId");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "Follows",
                newName: "PublicEventId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_EventId",
                table: "Follows",
                newName: "IX_Follows_PublicEventId");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "Comments",
                newName: "PublicEventId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_EventId",
                table: "Comments",
                newName: "IX_Comments_PublicEventId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PublicEvents",
                newName: "Promotor");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "PublicEvents",
                newName: "PublicEventId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_CategoryId",
                table: "PublicEvents",
                newName: "IX_PublicEvents_CategoryId");

            migrationBuilder.AlterColumn<int>(
                name: "Follows",
                table: "PublicEvents",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "PublicEvents",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Promotor",
                table: "PublicEvents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "PublicEvents",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublicEvents",
                table: "PublicEvents",
                column: "PublicEventId");

            migrationBuilder.CreateTable(
                name: "PrivateEvents",
                columns: table => new
                {
                    PrivateEventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Place = table.Column<string>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    Repeatable = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateEvents", x => x.PrivateEventId);
                    table.ForeignKey(
                        name: "FK_PrivateEvents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateEvents_UserId",
                table: "PrivateEvents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_PublicEvents_PublicEventId",
                table: "Comments",
                column: "PublicEventId",
                principalTable: "PublicEvents",
                principalColumn: "PublicEventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_PublicEvents_PublicEventId",
                table: "Follows",
                column: "PublicEventId",
                principalTable: "PublicEvents",
                principalColumn: "PublicEventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicEvents_Categories_CategoryId",
                table: "PublicEvents",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagEvents_PublicEvents_PublicEventId",
                table: "TagEvents",
                column: "PublicEventId",
                principalTable: "PublicEvents",
                principalColumn: "PublicEventId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_PublicEvents_PublicEventId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_PublicEvents_PublicEventId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicEvents_Categories_CategoryId",
                table: "PublicEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_TagEvents_PublicEvents_PublicEventId",
                table: "TagEvents");

            migrationBuilder.DropTable(
                name: "PrivateEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublicEvents",
                table: "PublicEvents");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "PublicEvents");

            migrationBuilder.RenameTable(
                name: "PublicEvents",
                newName: "Events");

            migrationBuilder.RenameColumn(
                name: "PublicEventId",
                table: "TagEvents",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_TagEvents_PublicEventId",
                table: "TagEvents",
                newName: "IX_TagEvents_EventId");

            migrationBuilder.RenameColumn(
                name: "PublicEventId",
                table: "Follows",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_PublicEventId",
                table: "Follows",
                newName: "IX_Follows_EventId");

            migrationBuilder.RenameColumn(
                name: "PublicEventId",
                table: "Comments",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_PublicEventId",
                table: "Comments",
                newName: "IX_Comments_EventId");

            migrationBuilder.RenameColumn(
                name: "Promotor",
                table: "Events",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "PublicEventId",
                table: "Events",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_PublicEvents_CategoryId",
                table: "Events",
                newName: "IX_Events_CategoryId");

            migrationBuilder.AlterColumn<int>(
                name: "Follows",
                table: "Events",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Events",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Events",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Events",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Repeatable",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TicketPrice",
                table: "Events",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId1",
                table: "Events",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Events_EventId",
                table: "Comments",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UserId1",
                table: "Events",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Events_EventId",
                table: "Follows",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagEvents_Events_EventId",
                table: "TagEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
