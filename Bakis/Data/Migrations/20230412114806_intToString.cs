using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakis.Data.Migrations
{
    public partial class intToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRooms_AspNetUsers_UserId1",
                table: "UserRooms");

            migrationBuilder.DropIndex(
                name: "IX_UserRooms_UserId1",
                table: "UserRooms");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserRooms");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserRooms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserRooms_UserId",
                table: "UserRooms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRooms_AspNetUsers_UserId",
                table: "UserRooms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRooms_AspNetUsers_UserId",
                table: "UserRooms");

            migrationBuilder.DropIndex(
                name: "IX_UserRooms_UserId",
                table: "UserRooms");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserRooms",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserRooms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRooms_UserId1",
                table: "UserRooms",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRooms_AspNetUsers_UserId1",
                table: "UserRooms",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
