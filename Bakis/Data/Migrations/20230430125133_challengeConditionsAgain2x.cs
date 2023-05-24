using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakis.Data.Migrations
{
    public partial class challengeConditionsAgain2x : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeCondition_Challenges_ChallengeId",
                table: "ChallengeCondition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChallengeCondition",
                table: "ChallengeCondition");

            migrationBuilder.RenameTable(
                name: "ChallengeCondition",
                newName: "ChallengeConditions");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeCondition_ChallengeId",
                table: "ChallengeConditions",
                newName: "IX_ChallengeConditions_ChallengeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChallengeConditions",
                table: "ChallengeConditions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeConditions_Challenges_ChallengeId",
                table: "ChallengeConditions",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallengeConditions_Challenges_ChallengeId",
                table: "ChallengeConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChallengeConditions",
                table: "ChallengeConditions");

            migrationBuilder.RenameTable(
                name: "ChallengeConditions",
                newName: "ChallengeCondition");

            migrationBuilder.RenameIndex(
                name: "IX_ChallengeConditions_ChallengeId",
                table: "ChallengeCondition",
                newName: "IX_ChallengeCondition_ChallengeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChallengeCondition",
                table: "ChallengeCondition",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallengeCondition_Challenges_ChallengeId",
                table: "ChallengeCondition",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
