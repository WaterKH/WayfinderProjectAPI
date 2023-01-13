using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class AddInterview2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_MA_Interview_InterviewId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_InterviewId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "InterviewId",
                table: "Games");

            migrationBuilder.CreateTable(
                name: "GameInterview",
                columns: table => new
                {
                    GamesId = table.Column<int>(type: "int", nullable: false),
                    InterviewsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameInterview", x => new { x.GamesId, x.InterviewsId });
                    table.ForeignKey(
                        name: "FK_GameInterview_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameInterview_MA_Interview_InterviewsId",
                        column: x => x.InterviewsId,
                        principalTable: "MA_Interview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GameInterview_InterviewsId",
                table: "GameInterview",
                column: "InterviewsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameInterview");

            migrationBuilder.AddColumn<int>(
                name: "InterviewId",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_InterviewId",
                table: "Games",
                column: "InterviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_MA_Interview_InterviewId",
                table: "Games",
                column: "InterviewId",
                principalTable: "MA_Interview",
                principalColumn: "Id");
        }
    }
}
