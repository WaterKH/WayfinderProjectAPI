using Discord;
using Discord.Commands;
using System.Web;
using WayfinderProjectAPI.Controllers;
using WayfinderProjectAPI.Data;

namespace WayfinderProject.Data.Discord
{
    [Group("ms")]
    public class MoogleShopModule : ModuleBase<SocketCommandContext>
    {
        private IServiceProvider serviceProvider;

        public MoogleShopModule(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [Command("random_recipe")]
        [Summary("Returns a random recipe.")]
        public async Task RandomRecipeAsync()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var randomRecipe = await api.GetRandomRecipe();
                    await Context.Channel.SendMessageAsync(
                        $"**Recipe Name**: {randomRecipe.Name} (Id: {randomRecipe.Id})\r\n" +
                        $"**Game**: {randomRecipe.Game.Name}\r\n" +
                        $"**Category**: {randomRecipe.Category}\r\n" +
                        $"**Recipe Materials**: {string.Join(", ", randomRecipe.RecipeMaterials.Select(x => $"x{x.Amount} {x.Inventory.Name}"))}\r\n" +
                        $"**Condition**: {randomRecipe.UnlockConditionDescription}\r\n" +
                        $"\r\nFind more information at https://wayfinderprojectkh.com/moogle_shop?category=Recipes&record={randomRecipe.Id}&open_row={randomRecipe.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("search_recipe")]
        [Summary("Returns recipes with search criteria.")]
        public async Task SearchRecipesAsync(string criteria)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var message = await Context.Channel.SendMessageAsync("Searching...");

                    var recipes = await api.GetRecipes("bfb7c747-6df4-40b5-9015-e36be500ab45", "", "", "", criteria);

                    var embedBuilder = new EmbedBuilder
                    {
                        Title = "Recipe Search Results",
                        Url = $"https://wayfinderprojectkh.com/memory_archive?category=Interviews&text={HttpUtility.UrlEncode(criteria)}",
                        Description = "Use the Ids of the search results to lookup the recipes you are looking for, for example **!ms recipe <recipe_id>**. Note: Limited to 25 results."
                    };

                    foreach (var scene in recipes.Take(25))
                    {
                        embedBuilder.AddField(new EmbedFieldBuilder
                        {
                            Name = scene.Id.ToString(),
                            Value = scene.Name
                        });
                    }

                    var embed = embedBuilder.Build();

                    await Context.Channel.ModifyMessageAsync(message.Id, m => { m.Embed = embed; m.Content = ""; });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("recipe")]
        [Summary("Returns recipe by id.")]
        public async Task GetRecipeAsync(int id)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var recipe = await api.GetRecipe(id);
                    await Context.Channel.SendMessageAsync(
                        $"**Recipe Name**: {recipe.Name} (Id: {recipe.Id})\r\n" +
                        $"**Game**: {recipe.Game.Name}\r\n" +
                        $"**Category**: {recipe.Category}\r\n" +
                        $"**Recipe Materials**: {string.Join(", ", recipe.RecipeMaterials.Select(x => $"x{x.Amount} {x.Inventory.Name}"))}\r\n" +
                        $"**Condition**: {recipe.UnlockConditionDescription}\r\n" +
                        $"\r\nFind more information at https://wayfinderprojectkh.com/moogle_shop?category=Recipes&record={recipe.Id}&open_row={recipe.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("random_record")]
        [Summary("Returns a random record.")]
        public async Task RandomRecordAsync()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var randomRecord = await api.GetRandomRecord();
                    await Context.Channel.SendMessageAsync(
                        $"**Record Name**: {randomRecord.Name} (Id: {randomRecord.Id})\r\n" +
                        $"**Game**: {randomRecord.Game.Name}\r\n" +
                        $"**Category**: {randomRecord.Category}\r\n" +
                        $"**Description**: {randomRecord.Description}\r\n" +
                        $"**Cost**: {randomRecord.Cost} {randomRecord.Currency}\r\n" +
                        $"**Drops**: {string.Join(", ", randomRecord.EnemyDrops.Select(x => $"x{x.DropRate} {x.CharacterLocation.Character.Name}"))}\r\n" +
                        $"**Additional Information**: {randomRecord.AdditionalInformation}\r\n" +
                        $"\r\nFind more information at https://wayfinderprojectkh.com/moogle_shop?category={HttpUtility.UrlEncode(randomRecord.Category)}&record={randomRecord.Id}&open_row={randomRecord.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("search_record")]
        [Summary("Returns records with search criteria.")]
        public async Task SearchRecordsAsync(string criteria)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var message = await Context.Channel.SendMessageAsync("Searching...");

                    var records = await api.GetInventory("bfb7c747-6df4-40b5-9015-e36be500ab45", "", "", "", "", "", criteria);

                    var embedBuilder = new EmbedBuilder
                    {
                        Title = "Record Search Results",
                        Url = $"https://wayfinderprojectkh.com/memory_archive?text={HttpUtility.UrlEncode(criteria)}",
                        Description = "Use the Ids of the search results to lookup the records you are looking for, for example **!ms record <record_id>**. Note: Limited to 25 results."
                    };

                    foreach (var scene in records.Take(25))
                    {
                        embedBuilder.AddField(new EmbedFieldBuilder
                        {
                            Name = scene.Id.ToString(),
                            Value = scene.Name
                        });
                    }

                    var embed = embedBuilder.Build();

                    await Context.Channel.ModifyMessageAsync(message.Id, m => { m.Embed = embed; m.Content = ""; });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("record")]
        [Summary("Returns record by id.")]
        public async Task GetRecordAsync(int id)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var record = await api.GetRecord(id);
                    await Context.Channel.SendMessageAsync(
                        $"**Record Name**: {record.Name} (Id: {record.Id})\r\n" +
                        $"**Game**: {record.Game.Name}\r\n" +
                        $"**Category**: {record.Category}\r\n" +
                        $"**Description**: {record.Description}\r\n" +
                        $"**Cost**: {record.Cost} {record.Currency}\r\n" +
                        $"**Drops**: {string.Join(", ", record.EnemyDrops.Select(x => $"x{x.DropRate} {x.CharacterLocation.Character.Name}"))}\r\n" +
                        $"**Additional Information**: {record.AdditionalInformation}\r\n" +
                        $"\r\nFind more information at https://wayfinderprojectkh.com/moogle_shop?category={HttpUtility.UrlEncode(record.Category)}&record={record.Id}&open_row={record.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}