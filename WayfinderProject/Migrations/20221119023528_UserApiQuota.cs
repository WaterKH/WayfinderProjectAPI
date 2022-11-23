using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class UserApiQuota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiCallQuota",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiCallQuota",
                table: "AspNetUsers");
        }
    }
}
