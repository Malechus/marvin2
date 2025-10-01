using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class moveabstractiontoperson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "DayOfMonth",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "Week",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "Chore");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "PersonChores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayOfMonth",
                table: "PersonChores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "PersonChores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "PersonChores",
                type: "varchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Week",
                table: "PersonChores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "priority",
                table: "PersonChores",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "PersonChores");

            migrationBuilder.DropColumn(
                name: "DayOfMonth",
                table: "PersonChores");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "PersonChores");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "PersonChores");

            migrationBuilder.DropColumn(
                name: "Week",
                table: "PersonChores");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "PersonChores");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "Chore",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayOfMonth",
                table: "Chore",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "Chore",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Chore",
                type: "varchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Week",
                table: "Chore",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "priority",
                table: "Chore",
                type: "int",
                nullable: true);
        }
    }
}
