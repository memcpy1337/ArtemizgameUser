using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Elo",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "Connected",
                table: "PlayerQueueData",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQueueData_MatchId",
                table: "PlayerQueueData",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQueueData_UserId",
                table: "PlayerQueueData",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerQueueData_MatchId",
                table: "PlayerQueueData");

            migrationBuilder.DropIndex(
                name: "IX_PlayerQueueData_UserId",
                table: "PlayerQueueData");

            migrationBuilder.DropColumn(
                name: "Connected",
                table: "PlayerQueueData");

            migrationBuilder.AddColumn<int>(
                name: "Elo",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
