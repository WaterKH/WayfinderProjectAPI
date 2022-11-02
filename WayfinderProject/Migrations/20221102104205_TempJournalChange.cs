using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    public partial class TempJournalChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreaScene_Scene_ScenesId",
                table: "AreaScene");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterScene_Scene_ScenesId",
                table: "CharacterScene");

            migrationBuilder.DropForeignKey(
                name: "FK_MusicScene_Scene_ScenesId",
                table: "MusicScene");

            migrationBuilder.DropForeignKey(
                name: "FK_Scene_Games_GameId",
                table: "Scene");

            migrationBuilder.DropForeignKey(
                name: "FK_Scene_Script_ScriptId",
                table: "Scene");

            migrationBuilder.DropForeignKey(
                name: "FK_SceneWorld_Scene_ScenesId",
                table: "SceneWorld");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scene",
                table: "Scene");

            migrationBuilder.RenameTable(
                name: "Scene",
                newName: "MA_Scene");

            migrationBuilder.RenameIndex(
                name: "IX_Scene_ScriptId",
                table: "MA_Scene",
                newName: "IX_MA_Scene_ScriptId");

            migrationBuilder.RenameIndex(
                name: "IX_Scene_GameId",
                table: "MA_Scene",
                newName: "IX_MA_Scene_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MA_Scene",
                table: "MA_Scene",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JJ_Character",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalInformation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JJ_Character", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JJ_Character_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JJ_Character_CharacterId",
                table: "JJ_Character",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaScene_MA_Scene_ScenesId",
                table: "AreaScene",
                column: "ScenesId",
                principalTable: "MA_Scene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterScene_MA_Scene_ScenesId",
                table: "CharacterScene",
                column: "ScenesId",
                principalTable: "MA_Scene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MA_Scene_Games_GameId",
                table: "MA_Scene",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MA_Scene_Script_ScriptId",
                table: "MA_Scene",
                column: "ScriptId",
                principalTable: "Script",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MusicScene_MA_Scene_ScenesId",
                table: "MusicScene",
                column: "ScenesId",
                principalTable: "MA_Scene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SceneWorld_MA_Scene_ScenesId",
                table: "SceneWorld",
                column: "ScenesId",
                principalTable: "MA_Scene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreaScene_MA_Scene_ScenesId",
                table: "AreaScene");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterScene_MA_Scene_ScenesId",
                table: "CharacterScene");

            migrationBuilder.DropForeignKey(
                name: "FK_MA_Scene_Games_GameId",
                table: "MA_Scene");

            migrationBuilder.DropForeignKey(
                name: "FK_MA_Scene_Script_ScriptId",
                table: "MA_Scene");

            migrationBuilder.DropForeignKey(
                name: "FK_MusicScene_MA_Scene_ScenesId",
                table: "MusicScene");

            migrationBuilder.DropForeignKey(
                name: "FK_SceneWorld_MA_Scene_ScenesId",
                table: "SceneWorld");

            migrationBuilder.DropTable(
                name: "JJ_Character");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MA_Scene",
                table: "MA_Scene");

            migrationBuilder.RenameTable(
                name: "MA_Scene",
                newName: "Scene");

            migrationBuilder.RenameIndex(
                name: "IX_MA_Scene_ScriptId",
                table: "Scene",
                newName: "IX_Scene_ScriptId");

            migrationBuilder.RenameIndex(
                name: "IX_MA_Scene_GameId",
                table: "Scene",
                newName: "IX_Scene_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scene",
                table: "Scene",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaScene_Scene_ScenesId",
                table: "AreaScene",
                column: "ScenesId",
                principalTable: "Scene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterScene_Scene_ScenesId",
                table: "CharacterScene",
                column: "ScenesId",
                principalTable: "Scene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MusicScene_Scene_ScenesId",
                table: "MusicScene",
                column: "ScenesId",
                principalTable: "Scene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scene_Games_GameId",
                table: "Scene",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scene_Script_ScriptId",
                table: "Scene",
                column: "ScriptId",
                principalTable: "Script",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SceneWorld_Scene_ScenesId",
                table: "SceneWorld",
                column: "ScenesId",
                principalTable: "Scene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
