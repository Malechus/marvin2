using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class tpt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Chore");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyChores");

            migrationBuilder.DropTable(
                name: "MonthlyChores");

            migrationBuilder.DropTable(
                name: "WeeklyChores");

            migrationBuilder.AddColumn<int>(
                name: "Day",
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
        }
    }
}
