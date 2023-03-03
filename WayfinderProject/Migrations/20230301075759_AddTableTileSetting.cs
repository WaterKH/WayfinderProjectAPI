using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class AddTableTileSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResultsDisplay",
                table: "SearchSettings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultsDisplay",
                table: "SearchSettings");
        }
    }
}
