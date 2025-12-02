using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Net.Code.AdventOfCode.Toolkit.Migrations
{
    /// <inheritdoc />
    public partial class Html : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Html",
                table: "Puzzles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Html",
                table: "Puzzles");
        }
    }
}
