using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class linkingtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chore_People_PersonId",
                table: "Chore");

            migrationBuilder.DropIndex(
                name: "IX_Chore_PersonId",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Chore");

            migrationBuilder.CreateTable(
                name: "PersonChores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    ChoreId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonChores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonChores_Chore_ChoreId",
                        column: x => x.ChoreId,
                        principalTable: "Chore",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonChores_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PersonChores_ChoreId",
                table: "PersonChores",
                column: "ChoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonChores_PersonId",
                table: "PersonChores",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonChores");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Chore",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Chore",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chore_PersonId",
                table: "Chore",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chore_People_PersonId",
                table: "Chore",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id");
        }
    }
}
