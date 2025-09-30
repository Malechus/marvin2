using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class _929sync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeeklyChore_Day",
                table: "Chore");

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "Chore",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "Chore");

            migrationBuilder.AddColumn<int>(
                name: "WeeklyChore_Day",
                table: "Chore",
                type: "int",
                nullable: true);
        }
    }
}
