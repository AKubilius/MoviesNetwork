using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakis.Data.Migrations
{
    public partial class requestId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "WatchingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "WatchingRequests");
        }
    }
}
