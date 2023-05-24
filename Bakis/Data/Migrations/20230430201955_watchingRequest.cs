using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakis.Data.Migrations
{
    public partial class watchingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatchingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InvitedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FriendId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WatchingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchingRequests_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchingRequests_AspNetUsers_InvitedById",
                        column: x => x.InvitedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchingRequests_FriendId",
                table: "WatchingRequests",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchingRequests_InvitedById",
                table: "WatchingRequests",
                column: "InvitedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchingRequests");
        }
    }
}
