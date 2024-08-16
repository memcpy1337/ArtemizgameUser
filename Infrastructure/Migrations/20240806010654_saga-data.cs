using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sagadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "PlayerQueueData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Port",
                table: "PlayerQueueData",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "PlayerQueueData");

            migrationBuilder.DropColumn(
                name: "Port",
                table: "PlayerQueueData");
        }
    }
}
