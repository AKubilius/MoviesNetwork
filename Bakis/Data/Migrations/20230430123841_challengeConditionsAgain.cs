using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakis.Data.Migrations
{
    public partial class challengeConditionsAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChallengeCondition_ChallengeId",
                table: "ChallengeCondition");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeCondition_ChallengeId",
                table: "ChallengeCondition",
                column: "ChallengeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChallengeCondition_ChallengeId",
                table: "ChallengeCondition");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeCondition_ChallengeId",
                table: "ChallengeCondition",
                column: "ChallengeId",
                unique: true);
        }
    }
}
