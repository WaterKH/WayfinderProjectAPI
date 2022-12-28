using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Areas_CharacterLocations_CharacterLocationId",
            //    table: "Areas");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_CharacterLocations_MS_EnemyDrop_EnemyDropId",
            //    table: "CharacterLocations");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Worlds_CharacterLocations_CharacterLocationId",
            //    table: "Worlds");

            //migrationBuilder.DropIndex(
            //    name: "IX_Worlds_CharacterLocationId",
            //    table: "Worlds");

            //migrationBuilder.DropIndex(
            //    name: "IX_CharacterLocations_EnemyDropId",
            //    table: "CharacterLocations");

            //migrationBuilder.DropIndex(
            //    name: "IX_Areas_CharacterLocationId",
            //    table: "Areas");

            //migrationBuilder.DropColumn(
            //    name: "CharacterLocationId",
            //    table: "Worlds");

            //migrationBuilder.DropColumn(
            //    name: "EnemyDropId",
            //    table: "CharacterLocations");

            //migrationBuilder.DropColumn(
            //    name: "CharacterLocationId",
            //    table: "Areas");

            //migrationBuilder.AddColumn<int>(
            //    name: "CharacterLocationId",
            //    table: "MS_EnemyDrop",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<string>(
            //    name: "EnemyName",
            //    table: "MS_EnemyDrop",
            //    type: "longtext",
            //    nullable: false)
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.AddColumn<int>(
            //    name: "WorldId",
            //    table: "CharacterLocations",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.CreateTable(
            //    name: "AreaCharacterLocation",
            //    columns: table => new
            //    {
            //        AreasId = table.Column<int>(type: "int", nullable: false),
            //        CharacterLocationsId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AreaCharacterLocation", x => new { x.AreasId, x.CharacterLocationsId });
            //        table.ForeignKey(
            //            name: "FK_AreaCharacterLocation_Areas_AreasId",
            //            column: x => x.AreasId,
            //            principalTable: "Areas",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_AreaCharacterLocation_CharacterLocations_CharacterLocationsId",
            //            column: x => x.CharacterLocationsId,
            //            principalTable: "CharacterLocations",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateIndex(
            //    name: "IX_MS_EnemyDrop_CharacterLocationId",
            //    table: "MS_EnemyDrop",
            //    column: "CharacterLocationId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CharacterLocations_WorldId",
            //    table: "CharacterLocations",
            //    column: "WorldId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AreaCharacterLocation_CharacterLocationsId",
            //    table: "AreaCharacterLocation",
            //    column: "CharacterLocationsId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_CharacterLocations_Worlds_WorldId",
            //    table: "CharacterLocations",
            //    column: "WorldId",
            //    principalTable: "Worlds",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MS_EnemyDrop_CharacterLocations_CharacterLocationId",
                table: "MS_EnemyDrop",
                column: "CharacterLocationId",
                principalTable: "CharacterLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterLocations_Worlds_WorldId",
                table: "CharacterLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_MS_EnemyDrop_CharacterLocations_CharacterLocationId",
                table: "MS_EnemyDrop");

            migrationBuilder.DropTable(
                name: "AreaCharacterLocation");

            migrationBuilder.DropIndex(
                name: "IX_MS_EnemyDrop_CharacterLocationId",
                table: "MS_EnemyDrop");

            migrationBuilder.DropIndex(
                name: "IX_CharacterLocations_WorldId",
                table: "CharacterLocations");

            migrationBuilder.DropColumn(
                name: "CharacterLocationId",
                table: "MS_EnemyDrop");

            migrationBuilder.DropColumn(
                name: "EnemyName",
                table: "MS_EnemyDrop");

            migrationBuilder.DropColumn(
                name: "WorldId",
                table: "CharacterLocations");

            migrationBuilder.AddColumn<int>(
                name: "CharacterLocationId",
                table: "Worlds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnemyDropId",
                table: "CharacterLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CharacterLocationId",
                table: "Areas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_CharacterLocationId",
                table: "Worlds",
                column: "CharacterLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLocations_EnemyDropId",
                table: "CharacterLocations",
                column: "EnemyDropId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CharacterLocationId",
                table: "Areas",
                column: "CharacterLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_CharacterLocations_CharacterLocationId",
                table: "Areas",
                column: "CharacterLocationId",
                principalTable: "CharacterLocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterLocations_MS_EnemyDrop_EnemyDropId",
                table: "CharacterLocations",
                column: "EnemyDropId",
                principalTable: "MS_EnemyDrop",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Worlds_CharacterLocations_CharacterLocationId",
                table: "Worlds",
                column: "CharacterLocationId",
                principalTable: "CharacterLocations",
                principalColumn: "Id");
        }
    }
}
