using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class HideAndSeekAdditional2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Character",
                table: "HideAndSeekData",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Character",
                table: "HideAndSeekData");
        }
    }
}
