using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class AddInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MA_Interaction",
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
                    table.PrimaryKey("PK_MA_Interaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MA_Interaction_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MA_Interaction_Script_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "Script",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AreaInteraction",
                columns: table => new
                {
                    AreasId = table.Column<int>(type: "int", nullable: false),
                    InteractionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaInteraction", x => new { x.AreasId, x.InteractionsId });
                    table.ForeignKey(
                        name: "FK_AreaInteraction_Areas_AreasId",
                        column: x => x.AreasId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaInteraction_MA_Interaction_InteractionsId",
                        column: x => x.InteractionsId,
                        principalTable: "MA_Interaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterInteraction",
                columns: table => new
                {
                    CharactersId = table.Column<int>(type: "int", nullable: false),
                    InteractionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInteraction", x => new { x.CharactersId, x.InteractionsId });
                    table.ForeignKey(
                        name: "FK_CharacterInteraction_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterInteraction_MA_Interaction_InteractionsId",
                        column: x => x.InteractionsId,
                        principalTable: "MA_Interaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InteractionMusic",
                columns: table => new
                {
                    InteractionsId = table.Column<int>(type: "int", nullable: false),
                    MusicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionMusic", x => new { x.InteractionsId, x.MusicId });
                    table.ForeignKey(
                        name: "FK_InteractionMusic_MA_Interaction_InteractionsId",
                        column: x => x.InteractionsId,
                        principalTable: "MA_Interaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InteractionMusic_Music_MusicId",
                        column: x => x.MusicId,
                        principalTable: "Music",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InteractionWorld",
                columns: table => new
                {
                    InteractionsId = table.Column<int>(type: "int", nullable: false),
                    WorldsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionWorld", x => new { x.InteractionsId, x.WorldsId });
                    table.ForeignKey(
                        name: "FK_InteractionWorld_MA_Interaction_InteractionsId",
                        column: x => x.InteractionsId,
                        principalTable: "MA_Interaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InteractionWorld_Worlds_WorldsId",
                        column: x => x.WorldsId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AreaInteraction_InteractionsId",
                table: "AreaInteraction",
                column: "InteractionsId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInteraction_InteractionsId",
                table: "CharacterInteraction",
                column: "InteractionsId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionMusic_MusicId",
                table: "InteractionMusic",
                column: "MusicId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionWorld_WorldsId",
                table: "InteractionWorld",
                column: "WorldsId");

            migrationBuilder.CreateIndex(
                name: "Index_InteractionName",
                table: "MA_Interaction",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MA_Interaction_GameId",
                table: "MA_Interaction",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MA_Interaction_ScriptId",
                table: "MA_Interaction",
                column: "ScriptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaInteraction");

            migrationBuilder.DropTable(
                name: "CharacterInteraction");

            migrationBuilder.DropTable(
                name: "InteractionMusic");

            migrationBuilder.DropTable(
                name: "InteractionWorld");

            migrationBuilder.DropTable(
                name: "MA_Interaction");
        }
    }
}
