using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class AddTrailers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MA_Trailer",
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
                    State = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    ScriptId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MA_Trailer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MA_Trailer_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MA_Trailer_Script_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "Script",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AreaTrailer",
                columns: table => new
                {
                    AreasId = table.Column<int>(type: "int", nullable: false),
                    TrailersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaTrailer", x => new { x.AreasId, x.TrailersId });
                    table.ForeignKey(
                        name: "FK_AreaTrailer_Areas_AreasId",
                        column: x => x.AreasId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaTrailer_MA_Trailer_TrailersId",
                        column: x => x.TrailersId,
                        principalTable: "MA_Trailer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterTrailer",
                columns: table => new
                {
                    CharactersId = table.Column<int>(type: "int", nullable: false),
                    TrailersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTrailer", x => new { x.CharactersId, x.TrailersId });
                    table.ForeignKey(
                        name: "FK_CharacterTrailer_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterTrailer_MA_Trailer_TrailersId",
                        column: x => x.TrailersId,
                        principalTable: "MA_Trailer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MusicTrailer",
                columns: table => new
                {
                    MusicId = table.Column<int>(type: "int", nullable: false),
                    TrailersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicTrailer", x => new { x.MusicId, x.TrailersId });
                    table.ForeignKey(
                        name: "FK_MusicTrailer_MA_Trailer_TrailersId",
                        column: x => x.TrailersId,
                        principalTable: "MA_Trailer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusicTrailer_Music_MusicId",
                        column: x => x.MusicId,
                        principalTable: "Music",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrailerWorld",
                columns: table => new
                {
                    TrailersId = table.Column<int>(type: "int", nullable: false),
                    WorldsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrailerWorld", x => new { x.TrailersId, x.WorldsId });
                    table.ForeignKey(
                        name: "FK_TrailerWorld_MA_Trailer_TrailersId",
                        column: x => x.TrailersId,
                        principalTable: "MA_Trailer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrailerWorld_Worlds_WorldsId",
                        column: x => x.WorldsId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AreaTrailer_TrailersId",
                table: "AreaTrailer",
                column: "TrailersId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTrailer_TrailersId",
                table: "CharacterTrailer",
                column: "TrailersId");

            migrationBuilder.CreateIndex(
                name: "Index_TrailerName",
                table: "MA_Trailer",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MA_Trailer_GameId",
                table: "MA_Trailer",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MA_Trailer_ScriptId",
                table: "MA_Trailer",
                column: "ScriptId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicTrailer_TrailersId",
                table: "MusicTrailer",
                column: "TrailersId");

            migrationBuilder.CreateIndex(
                name: "IX_TrailerWorld_WorldsId",
                table: "TrailerWorld",
                column: "WorldsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaTrailer");

            migrationBuilder.DropTable(
                name: "CharacterTrailer");

            migrationBuilder.DropTable(
                name: "MusicTrailer");

            migrationBuilder.DropTable(
                name: "TrailerWorld");

            migrationBuilder.DropTable(
                name: "MA_Trailer");
        }
    }
}
