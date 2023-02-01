using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountUser2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatreonUserId",
                table: "AspNetUsers",
                newName: "PatreonRefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "PatreonAccessToken",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatreonAccessToken",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PatreonRefreshToken",
                table: "AspNetUsers",
                newName: "PatreonUserId");
        }
    }
}
