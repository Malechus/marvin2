using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class tph : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyChores");

            migrationBuilder.DropTable(
                name: "MonthlyChores");

            migrationBuilder.DropTable(
                name: "WeeklyChores");

            migrationBuilder.DropColumn(
                name: "PersonName",
                table: "Chore");

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
                name: "WeeklyChore_Day",
                table: "Chore",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "priority",
                table: "Chore",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "DayOfMonth",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "Week",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "WeeklyChore_Day",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "Chore");

            migrationBuilder.AddColumn<string>(
                name: "PersonName",
                table: "Chore",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DailyChores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyChores_Chore_Id",
                        column: x => x.Id,
                        principalTable: "Chore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MonthlyChores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyChores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyChores_Chore_Id",
                        column: x => x.Id,
                        principalTable: "Chore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WeeklyChores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyChores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyChores_Chore_Id",
                        column: x => x.Id,
                        principalTable: "Chore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
