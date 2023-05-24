using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakis.Data.Migrations
{
    public partial class ChallengesChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChallenges_AspNetUsers_UserId1",
                table: "UserChallenges");

            migrationBuilder.DropIndex(
                name: "IX_UserChallenges_UserId1",
                table: "UserChallenges");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserChallenges");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserChallenges",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_UserId",
                table: "UserChallenges",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallenges_AspNetUsers_UserId",
                table: "UserChallenges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChallenges_AspNetUsers_UserId",
                table: "UserChallenges");

            migrationBuilder.DropIndex(
                name: "IX_UserChallenges_UserId",
                table: "UserChallenges");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserChallenges",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserChallenges",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_UserId1",
                table: "UserChallenges",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallenges_AspNetUsers_UserId1",
                table: "UserChallenges",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
