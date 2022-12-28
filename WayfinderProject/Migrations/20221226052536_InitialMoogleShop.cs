using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WayfinderProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialMoogleShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CharacterLocationId",
                table: "Worlds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CharacterLocationId",
                table: "Areas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MS_Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalInformation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cost = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MS_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MS_Inventory_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MS_Recipe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnlockConditionDescription = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MS_Recipe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MS_Recipe_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MS_EnemyDrop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DropRate = table.Column<float>(type: "float", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MS_EnemyDrop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MS_EnemyDrop_MS_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "MS_Inventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MS_RecipeMaterial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MS_RecipeMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MS_RecipeMaterial_MS_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "MS_Inventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MS_RecipeMaterial_MS_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "MS_Recipe",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    EnemyDropId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterLocations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLocations_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLocations_MS_EnemyDrop_EnemyDropId",
                        column: x => x.EnemyDropId,
                        principalTable: "MS_EnemyDrop",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_CharacterLocationId",
                table: "Worlds",
                column: "CharacterLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CharacterLocationId",
                table: "Areas",
                column: "CharacterLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLocations_CharacterId",
                table: "CharacterLocations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLocations_EnemyDropId",
                table: "CharacterLocations",
                column: "EnemyDropId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLocations_GameId",
                table: "CharacterLocations",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MS_EnemyDrop_InventoryId",
                table: "MS_EnemyDrop",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MS_Inventory_GameId",
                table: "MS_Inventory",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MS_Recipe_GameId",
                table: "MS_Recipe",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MS_RecipeMaterial_InventoryId",
                table: "MS_RecipeMaterial",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MS_RecipeMaterial_RecipeId",
                table: "MS_RecipeMaterial",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_CharacterLocations_CharacterLocationId",
                table: "Areas",
                column: "CharacterLocationId",
                principalTable: "CharacterLocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Worlds_CharacterLocations_CharacterLocationId",
                table: "Worlds",
                column: "CharacterLocationId",
                principalTable: "CharacterLocations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_CharacterLocations_CharacterLocationId",
                table: "Areas");

            migrationBuilder.DropForeignKey(
                name: "FK_Worlds_CharacterLocations_CharacterLocationId",
                table: "Worlds");

            migrationBuilder.DropTable(
                name: "CharacterLocations");

            migrationBuilder.DropTable(
                name: "MS_RecipeMaterial");

            migrationBuilder.DropTable(
                name: "MS_EnemyDrop");

            migrationBuilder.DropTable(
                name: "MS_Recipe");

            migrationBuilder.DropTable(
                name: "MS_Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Worlds_CharacterLocationId",
                table: "Worlds");

            migrationBuilder.DropIndex(
                name: "IX_Areas_CharacterLocationId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "CharacterLocationId",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "CharacterLocationId",
                table: "Areas");
        }
    }
}
