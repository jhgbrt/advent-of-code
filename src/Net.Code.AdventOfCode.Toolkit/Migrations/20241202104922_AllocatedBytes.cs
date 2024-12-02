using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Net.Code.AdventOfCode.Toolkit.Migrations
{
    /// <inheritdoc />
    public partial class AllocatedBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Part1_bytes",
                table: "Results",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Part2_bytes",
                table: "Results",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Part1_bytes",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "Part2_bytes",
                table: "Results");
        }
    }
}
