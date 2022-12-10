using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class JJUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "JJ_Character");

            //migrationBuilder.CreateTable(
            //    name: "DailyCutscenes",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        DateCode = table.Column<string>(type: "varchar(255)", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        SceneId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DailyCutscenes", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JJ_Entry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalInformation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JJ_Entry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JJ_Entry_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterJournalEntry",
                columns: table => new
                {
                    CharactersId = table.Column<int>(type: "int", nullable: false),
                    JournalEntriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterJournalEntry", x => new { x.CharactersId, x.JournalEntriesId });
                    table.ForeignKey(
                        name: "FK_CharacterJournalEntry_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterJournalEntry_JJ_Entry_JournalEntriesId",
                        column: x => x.JournalEntriesId,
                        principalTable: "JJ_Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalEntryWorld",
                columns: table => new
                {
                    JournalEntriesId = table.Column<int>(type: "int", nullable: false),
                    WorldsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryWorld", x => new { x.JournalEntriesId, x.WorldsId });
                    table.ForeignKey(
                        name: "FK_JournalEntryWorld_JJ_Entry_JournalEntriesId",
                        column: x => x.JournalEntriesId,
                        principalTable: "JJ_Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryWorld_Worlds_WorldsId",
                        column: x => x.WorldsId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterJournalEntry_JournalEntriesId",
                table: "CharacterJournalEntry",
                column: "JournalEntriesId");

            //migrationBuilder.CreateIndex(
            //    name: "Index_DateCode",
            //    table: "DailyCutscenes",
            //    column: "DateCode");

            migrationBuilder.CreateIndex(
                name: "IX_JJ_Entry_GameId",
                table: "JJ_Entry",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryWorld_WorldsId",
                table: "JournalEntryWorld",
                column: "WorldsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterJournalEntry");

            migrationBuilder.DropTable(
                name: "DailyCutscenes");

            migrationBuilder.DropTable(
                name: "JournalEntryWorld");

            migrationBuilder.DropTable(
                name: "JJ_Entry");

            migrationBuilder.CreateTable(
                name: "JJ_Character",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "longtext", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_JJ_Character_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JJ_Character_CharacterId",
                table: "JJ_Character",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_JJ_Character_GameId",
                table: "JJ_Character",
                column: "GameId");
        }
    }
}
