using Microsoft.EntityFrameworkCore.Migrations;

namespace PrzyjaznyBot.Migrations
{
    public partial class PrzyjaznyBotDbv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Users",
                newName: "LastDailyRewardClaimDateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastDailyRewardClaimDateTime",
                table: "Users",
                newName: "DateTime");
        }
    }
}
