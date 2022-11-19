using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Music",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Link = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Music", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Script",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SceneName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Script", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Worlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JJ_Character",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalInformation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_JJ_Character_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MA_Scene",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Link = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    ScriptId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MA_Scene", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MA_Scene_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MA_Scene_Script_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "Script",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScriptLine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Character = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Line = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ScriptId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptLine_Script_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "Script",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AreaScene",
                columns: table => new
                {
                    AreasId = table.Column<int>(type: "int", nullable: false),
                    ScenesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaScene", x => new { x.AreasId, x.ScenesId });
                    table.ForeignKey(
                        name: "FK_AreaScene_Areas_AreasId",
                        column: x => x.AreasId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaScene_MA_Scene_ScenesId",
                        column: x => x.ScenesId,
                        principalTable: "MA_Scene",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterScene",
                columns: table => new
                {
                    CharactersId = table.Column<int>(type: "int", nullable: false),
                    ScenesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterScene", x => new { x.CharactersId, x.ScenesId });
                    table.ForeignKey(
                        name: "FK_CharacterScene_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterScene_MA_Scene_ScenesId",
                        column: x => x.ScenesId,
                        principalTable: "MA_Scene",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MusicScene",
                columns: table => new
                {
                    MusicId = table.Column<int>(type: "int", nullable: false),
                    ScenesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicScene", x => new { x.MusicId, x.ScenesId });
                    table.ForeignKey(
                        name: "FK_MusicScene_MA_Scene_ScenesId",
                        column: x => x.ScenesId,
                        principalTable: "MA_Scene",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusicScene_Music_MusicId",
                        column: x => x.MusicId,
                        principalTable: "Music",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SceneWorld",
                columns: table => new
                {
                    ScenesId = table.Column<int>(type: "int", nullable: false),
                    WorldsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneWorld", x => new { x.ScenesId, x.WorldsId });
                    table.ForeignKey(
                        name: "FK_SceneWorld_MA_Scene_ScenesId",
                        column: x => x.ScenesId,
                        principalTable: "MA_Scene",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SceneWorld_Worlds_WorldsId",
                        column: x => x.WorldsId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "Index_AreaName",
                table: "Areas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AreaScene_ScenesId",
                table: "AreaScene",
                column: "ScenesId");

            migrationBuilder.CreateIndex(
                name: "Index_CharacterName",
                table: "Characters",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterScene_ScenesId",
                table: "CharacterScene",
                column: "ScenesId");

            migrationBuilder.CreateIndex(
                name: "Index_GameName",
                table: "Games",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_JJ_Character_CharacterId",
                table: "JJ_Character",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JJ_Character_GameId",
                table: "JJ_Character",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "Index_SceneName",
                table: "MA_Scene",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MA_Scene_GameId",
                table: "MA_Scene",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MA_Scene_ScriptId",
                table: "MA_Scene",
                column: "ScriptId");

            migrationBuilder.CreateIndex(
                name: "Index_MusicName",
                table: "Music",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MusicScene_ScenesId",
                table: "MusicScene",
                column: "ScenesId");

            migrationBuilder.CreateIndex(
                name: "IX_SceneWorld_WorldsId",
                table: "SceneWorld",
                column: "WorldsId");

            migrationBuilder.CreateIndex(
                name: "Index_GameSceneName",
                table: "Script",
                columns: new[] { "GameName", "SceneName" });

            migrationBuilder.CreateIndex(
                name: "IX_ScriptLine_ScriptId",
                table: "ScriptLine",
                column: "ScriptId");

            migrationBuilder.CreateIndex(
                name: "Index_WorldName",
                table: "Worlds",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaScene");

            migrationBuilder.DropTable(
                name: "CharacterScene");

            migrationBuilder.DropTable(
                name: "JJ_Character");

            migrationBuilder.DropTable(
                name: "MusicScene");

            migrationBuilder.DropTable(
                name: "SceneWorld");

            migrationBuilder.DropTable(
                name: "ScriptLine");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Music");

            migrationBuilder.DropTable(
                name: "MA_Scene");

            migrationBuilder.DropTable(
                name: "Worlds");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Script");
        }
    }
}
