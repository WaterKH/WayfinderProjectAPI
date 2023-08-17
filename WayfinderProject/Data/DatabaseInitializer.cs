﻿using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Data
{
    public static class DatabaseInitializer
    {
        public static bool IsInitializing { get; set; } = false;

        public static void Initialize(WayfinderContext context)
        {
            if (IsInitializing)
                return;

            IsInitializing = true;

            //context.Database.Migrate();

            // MA
            // Load Areas Data into Database
            //CreateAreas(context);

            //// Load Character Data into Database
            //CreateCharacters(context);

            // Character Locations
            //CreateEnemyLocations(context);

            //// Load Game Data into Database
            //CreateGames(context);

            //// Load Music Data into Database
            //CreateMusic(context);

            //// Load World Data into Database
            //CreateWorlds(context);

            //// Load Scene Data into Database
            //CreateScenes(context);

            //// Load Script Data into Database
            //CreateScripts(context);

            //CreateAddendum(context);

            // Load Interviews
            //CreateInterviews(context);

            // Load Interactions
            //CreateInteractions(context);

            // JJ
            // Character
            //CreateCharacterEntries(context);

            // Story
            //CreateStoryEntries(context);

            // Enemy
            //CreateEnemiesEntries(context);

            // Report
            //CreateReportEntries(context);

            // MS
            // Inventory
            //CreateInventory(context);

            // Recipes
            //CreateRecipes(context);


            IsInitializing = false;
        }

        #region Memory Archive
        public static void CreateAreas(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/_areas.json"));
            var areas = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(streamReader.ReadToEnd());

            if (!areas!.ContainsKey("Areas"))
                throw new Exception("No Areas List Found!");

            foreach (var area in areas["Areas"])
                context.Areas.Add(new Area { Name = area });

            context.SaveChanges();
        }

        public static void CreateCharacters(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/_characters.json"));
            var characters = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(streamReader.ReadToEnd());

            if (!characters!.ContainsKey("Characters"))
                throw new Exception("No Characters List Found!");

            foreach (var character in characters["Characters"])
                context.Characters.Add(new Character { Name = character });

            context.SaveChanges();
        }

        public static void CreateGames(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/_games.json"));
            var games = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(streamReader.ReadToEnd());

            if (!games!.ContainsKey("Games"))
                throw new Exception("No Games List Found!");

            foreach (var game in games["Games"])
                context.Games.Add(new Game { Name = game });

            context.SaveChanges();
        }

        class MusicObject
        {
            public string Name { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;
        }

        public static void CreateMusic(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/_music.json"));
            var music = JsonSerializer.Deserialize<Dictionary<string, List<MusicObject>>>(streamReader.ReadToEnd());

            if (!music!.ContainsKey("Music"))
                throw new Exception("No Music List Found!");

            foreach (var song in music["Music"])
                context.Music.Add(new Music { Name = song.Name, Link = song.Link });

            context.SaveChanges();
        }

        public static void CreateWorlds(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/_worlds.json"));
            var worlds = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(streamReader.ReadToEnd());

            if (!worlds!.ContainsKey("Worlds"))
                throw new Exception("No Worlds List Found!");

            foreach (var world in worlds["Worlds"])
                context.Worlds.Add(new World { Name = world });

            context.SaveChanges();
        }

        public static void CreateScripts(WayfinderContext context)
        {
            CreateScript(context, "_khx_lines.json", "Kingdom Hearts χ");
            CreateScript(context, "_kh_unchainedx_lines.json", "Kingdom Hearts Unchained χ");
            CreateScript(context, "_khxbc_lines.json", "Kingdom Hearts χ Back Cover");
            CreateScript(context, "_khux_lines.json", "Kingdom Hearts Union χ");
            CreateScript(context, "_khbbs_lines.json", "Kingdom Hearts Birth By Sleep");
            CreateScript(context, "_kh0_2_lines.json", "Kingdom Hearts 0.2");
            CreateScript(context, "_kh1_lines.json", "Kingdom Hearts");
            CreateScript(context, "_khdays_lines.json", "Kingdom Hearts 358/2 Days");
            CreateScript(context, "_khrecom_lines.json", "Kingdom Hearts Re:Chain of Memories");
            CreateScript(context, "_kh2_lines.json", "Kingdom Hearts II");
            CreateScript(context, "_khrecoded_lines.json", "Kingdom Hearts Re:Coded");
            CreateScript(context, "_kh3d_lines.json", "Kingdom Hearts Dream Drop Distance");
            CreateScript(context, "_kh3_lines.json", "Kingdom Hearts III");
            CreateScript(context, "_khdr_lines.json", "Kingdom Hearts Dark Road");
            CreateScript(context, "_khmom_lines.json", "Kingdom Hearts Melody of Memory");
        }

        class LineScriptObject
        {
            public int Order { get; set; }
            public string Character { get; set; } = string.Empty;
            public string Line { get; set; } = string.Empty;
        }

        public static void CreateScript(WayfinderContext context, string fileName, string gameName)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @$"wwwroot/data/seed/scripts/{fileName}"));
            var script = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<LineScriptObject>>>>(streamReader.ReadToEnd());

            if (!script!.ContainsKey("Script"))
                throw new Exception("No Script List Found!");

            foreach (var (scene, lines) in script["Script"])
            {
                var temp = new Script { SceneName = scene, GameName = gameName, Lines = new List<ScriptLine>() };
                foreach (var line in lines)
                {
                    temp.Lines.Add(new ScriptLine { Order = line.Order, Character = line.Character, Line = line.Line });
                }

                context.Script.Add(temp);
            }


            context.SaveChanges();
        }

        class SceneObject
        {
            public string Name { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;
            public List<string> Worlds { get; set; } = new List<string>();
            public List<string> Characters { get; set; } = new List<string>();
            public List<string> Areas { get; set; } = new List<string>();
            public List<string> Music { get; set; } = new List<string>();
        }

        public static void CreateScenes(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/_scenes.json"));
            var allScenes = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<SceneObject>>>>(streamReader.ReadToEnd());

            if (!allScenes!.ContainsKey("Scenes"))
                throw new Exception("No Scenes List Found!");

            foreach (var (gameName, scenes) in allScenes["Scenes"])
            {
                //if (gameName != "Kingdom Hearts") break;

                foreach (var scene in scenes)
                {
                    if (context.Scenes.Any(x => x.Game.Name == gameName && x.Name == scene.Name))
                        continue;

                    var game = context.Games.FirstOrDefault(x => x.Name == gameName);
                    var worlds = context.Worlds.Where(x => scene.Worlds.Contains(x.Name)).ToList();
                    var characters = context.Characters.Where(x => scene.Characters.Contains(x.Name)).ToList();
                    var areas = context.Areas.Where(x => scene.Areas.Contains(x.Name)).ToList();
                    var music = context.Music.Where(x => scene.Music.Contains(x.Name)).ToList();
                    var script = context.Script.FirstOrDefault(x => x.SceneName == scene.Name && x.GameName == gameName) ??
                        context.Script.FirstOrDefault(x => x.SceneName == "None");

                    context.Scenes.Add(new Scene
                    {
                        Game = game,
                        Name = scene.Name,
                        Link = scene.Link,
                        Worlds = worlds,
                        Characters = characters,
                        Areas = areas,
                        Music = music,
                        Script = script
                    });
                }
            }

            context.SaveChanges();
        }

        public static void CreateAddendum(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/scripts/_khrecom_addendum_lines.json"));
            var allAddendum = JsonSerializer.Deserialize<Dictionary<string, List<InteractionObject>>>(streamReader.ReadToEnd());

            if (!allAddendum!.ContainsKey("Addendum"))
                throw new Exception("No Addendum List Found!");

            var allowed = new List<string> { "Beware Your Memories", "Rabbit's Veggies", "Friends Forever (Alternate)", "Friends on the Islands" };
            foreach (var addendum in allAddendum["Addendum"])
            {
                if (!allowed.Contains(addendum.Title)) continue;

                //if (gameName != "Kingdom Hearts") break;

                var gameName = "Kingdom Hearts Re:Chain of Memories";

                // Create Script
                if (!context.Script.Any(x => x.GameName == gameName && x.SceneName == addendum.Title))
                {
                    context.Script.Add(new Script
                    {
                        GameName = gameName,
                        SceneName = addendum.Title,
                        Lines = addendum.Interactions.Select(x => new ScriptLine { Character = x.Character, Line = x.Line, Order = x.Order }).ToList()
                    });

                    context.SaveChanges();
                }

                var scene = context.Scenes.FirstOrDefault(x => x.Game.Name == gameName && x.Name == addendum.Title);

                var script = context.Script.FirstOrDefault(x => x.SceneName == addendum.Title && x.GameName == gameName) ??
                    context.Script.FirstOrDefault(x => x.SceneName == "None");

                if (scene != null)
                {
                    scene.Script = script;
                }
                else
                {
                    try
                    {
                        var game = context.Games.FirstOrDefault(x => x.Name == gameName);
                        var worlds = context.Worlds.Where(x => addendum.Worlds.Contains(x.Name)).ToList();
                        var characters = context.Characters.Where(x => addendum.Characters.Contains(x.Name)).ToList();
                        var areas = context.Areas.Where(x => addendum.Areas.Contains(x.Name)).ToList();
                        var music = context.Music.Where(x => addendum.Music.Contains(x.Name)).ToList();

                        context.Scenes.Add(new Scene
                        {
                            Game = game,
                            Name = addendum.Title,
                            Link = addendum.Link,
                            Worlds = worlds,
                            Characters = characters,
                            Areas = areas,
                            Music = music,
                            Script = script
                        });
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(addendum.Title);
                    }
                }
            }

            context.SaveChanges();
        }

        class InterviewObject
        {
            public string Name { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;
            public DateTime ReleaseDate { get; set; }
            public string AdditionalLink { get; set; } = string.Empty;

            public List<InterviewLineObject> Conversation { get; set; } = new List<InterviewLineObject>();
            public List<string> GameNames { get; set; } = new List<string>();
            public List<string> Participants { get; set; } = new List<string>();
            public string Provider { get; set; } = string.Empty;
            public string Translator { get; set; } = string.Empty;
        }
        class InterviewLineObject
        {
            public int Order { get; set; }
            public string Speaker { get; set; } = string.Empty;
            public string Line { get; set; } = string.Empty;
        }
        public static void CreateInterviews(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/ma/_kh1_interviews.json"));
            var allInterviews = JsonSerializer.Deserialize<Dictionary<string, List<InterviewObject>>>(streamReader.ReadToEnd());

            if (!allInterviews!.ContainsKey("Interviews"))
                throw new Exception("No Interviews List Found!");

            foreach (var interview in allInterviews["Interviews"])
            {
                var games = context.Games.Where(x => interview.GameNames.Contains(x.Name));
                var conversation = interview.Conversation.Select(x => new InterviewLine { Speaker = x.Speaker, Line = x.Line, Order = x.Order });

                var participants = new List<Participant>();
                interview.Participants.ForEach(x =>
                {
                    var participant = context.Participants.FirstOrDefault(y => y.Name == x);

                    if (participant == null)
                    {
                        context.Participants.Add(new Participant { Name = x });
                        context.SaveChanges();

                        participant = context.Participants.First(y => y.Name == x);
                    }

                    participants.Add(participant);
                });

                var provider = context.Providers.FirstOrDefault(x => x.Name == interview.Provider);
                if (provider == null)
                {
                    context.Providers.Add(new Provider { Name = interview.Provider });
                    context.SaveChanges();

                    provider = context.Providers.First(x => x.Name == interview.Provider);
                }

                var translator = context.Translators.FirstOrDefault(x => x.Name == interview.Translator);
                if (translator == null)
                {
                    context.Translators.Add(new Translator { Name = interview.Translator });
                    context.SaveChanges();

                    translator = context.Translators.First(x => x.Name == interview.Translator);
                }


                context.Interviews.Add(new Interview
                {
                    Name = interview.Name,
                    Link = interview.Link,
                    ReleaseDate = interview.ReleaseDate,
                    AdditionalLink = interview.AdditionalLink,

                    Conversation = conversation.ToList(),
                    Games = games.ToList(),
                    Participants = participants,
                    Provider = provider,
                    Translator = translator
                });

            }

            context.SaveChanges();
        }

        class InteractionObject
        {
            public string Title { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;

            public List<InteractionLineObject> Interactions { get; set; } = new List<InteractionLineObject>();
            public List<string> Worlds { get; set; } = new List<string>();
            public List<string> Characters { get; set; } = new List<string>();
            public List<string> Areas { get; set; } = new List<string>();
            public List<string> Music { get; set; } = new List<string>();
        }
        class InteractionLineObject
        {
            public int Order { get; set; }
            public string Character { get; set; } = string.Empty;
            public string Line { get; set; } = string.Empty;
        }
        public static void CreateInteractions(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/ma/interactions/_khrecom_interactions.json"));
            var allInteractions = JsonSerializer.Deserialize<Dictionary<string, List<InteractionObject>>>(streamReader.ReadToEnd());

            if (!allInteractions!.ContainsKey("Interactions"))
                throw new Exception("No Interactions List Found!");

            foreach (var interaction in allInteractions["Interactions"])
            {
                //if (gameName != "Kingdom Hearts") break;

                var gameName = "Kingdom Hearts Re:Chain of Memories";

                // Create Script
                context.Script.Add(new Script
                {
                    GameName = gameName,
                    SceneName = interaction.Title,
                    Lines = interaction.Interactions.Select(x => new ScriptLine { Character = x.Character, Line = x.Line, Order = x.Order }).ToList()
                });

                context.SaveChanges();

                // Add missing characters
                foreach (var character in interaction.Characters)
                {
                    if (context.Characters.FirstOrDefault(x => x.Name == character) == null)
                    {
                        context.Characters.Add(new Character
                        {
                            Name = character
                        });
                    }
                }

                var game = context.Games.FirstOrDefault(x => x.Name == gameName);
                var worlds = context.Worlds.Where(x => interaction.Worlds.Contains(x.Name)).ToList();
                var characters = context.Characters.Where(x => interaction.Characters.Contains(x.Name)).ToList();
                var areas = context.Areas.Where(x => interaction.Areas.Contains(x.Name)).ToList();
                var music = context.Music.Where(x => interaction.Music.Contains(x.Name)).ToList();
                var script = context.Script.FirstOrDefault(x => x.SceneName == interaction.Title && x.GameName == gameName) ??
                    context.Script.FirstOrDefault(x => x.SceneName == "None");

                context.Interactions.Add(new Interaction
                {
                    Game = game,
                    Name = interaction.Title,
                    Link = interaction.Link,
                    Worlds = worlds,
                    Characters = characters,
                    Areas = areas,
                    Music = music,
                    Script = script
                });

            }

            context.SaveChanges();
        }
        #endregion Memory Archive


        #region Jiminy's Journal
        public class JJCharacterObject
        {
            public string Title { get; set; } = string.Empty;
            public List<string> Characters { get; set; } = new List<string>();
            public List<string> Worlds { get; set; } = new List<string>();
            public string Description { get; set; } = string.Empty;
            public string AdditionalInformation { get; set; } = string.Empty;
        }
        public static void CreateCharacterEntries(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/jj/_jj_characters.json"));
            var allJJCharacters = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<JJCharacterObject>>>>(streamReader.ReadToEnd());

            if (!allJJCharacters!.ContainsKey("Characters"))
                throw new Exception("No Characters List Found!");

            foreach (var (gameName, characters) in allJJCharacters["Characters"])
            {
                if (gameName == "Kingdom Hearts Re:Chain of Memories" || gameName == "Kingdom Hearts") continue;

                foreach (var character in characters)
                {
                    var tempCharacters = context.Characters.Where(x => character.Characters.Contains(x.Name)).ToList();
                    if (!tempCharacters.Any())
                        Console.WriteLine();

                    var tempWorlds = context.Worlds.Where(x => character.Worlds.Contains(x.Name)).ToList();
                    if (!tempWorlds.Any())
                        Console.WriteLine();

                    context.JournalEntries.Add(new JournalEntry
                    {
                        Title = character.Title,
                        Characters = tempCharacters,
                        Worlds = tempWorlds,
                        Description = character.Description,
                        AdditionalInformation = character.AdditionalInformation,
                        Game = context.Games.FirstOrDefault(x => x.Name == gameName) ?? new Game(),
                        Category = "Character"
                    });
                }
            }

            context.SaveChanges();
        }

        public static void CreateStoryEntries(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/jj/_jj_story.json"));
            var allJJStoryEntries = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<JJCharacterObject>>>>(streamReader.ReadToEnd());

            if (!allJJStoryEntries!.ContainsKey("Story"))
                throw new Exception("No Story List Found!");

            foreach (var (gameName, storyEntries) in allJJStoryEntries["Story"])
            {
                if (gameName == "Kingdom Hearts") continue;

                foreach (var storyEntry in storyEntries)
                {
                    var tempCharacters = context.Characters.Where(x => storyEntry.Characters.Contains(x.Name)).ToList();
                    if (!tempCharacters.Any())
                        Console.WriteLine();

                    var tempWorlds = context.Worlds.Where(x => storyEntry.Worlds.Contains(x.Name)).ToList();
                    if (!tempWorlds.Any())
                        Console.WriteLine();

                    context.JournalEntries.Add(new JournalEntry
                    {
                        Title = storyEntry.Title,
                        Characters = tempCharacters,
                        Worlds = tempWorlds,
                        Description = storyEntry.Description,
                        AdditionalInformation = storyEntry.AdditionalInformation,
                        Game = context.Games.FirstOrDefault(x => x.Name == gameName) ?? new Game(),
                        Category = "Story"
                    });
                }
            }

            context.SaveChanges();
        }

        public static void CreateEnemiesEntries(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/jj/_jj_enemies.json"));
            var allJJEnemiesEntries = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<JJCharacterObject>>>>(streamReader.ReadToEnd());

            if (!allJJEnemiesEntries!.ContainsKey("Enemies"))
                throw new Exception("No Enemy List Found!");

            foreach (var (gameName, enemyEntries) in allJJEnemiesEntries["Enemies"])
            {
                if (gameName == "Kingdom Hearts") continue;

                foreach (var enemyEntry in enemyEntries)
                {

                    enemyEntry.Characters.ForEach(x =>
                    {
                        if (!context.Characters.Any(y => y.Name == x))
                        {
                            context.Characters.Add(new Character { Name = x });
                            context.SaveChanges();
                        }
                    });
                    var tempCharacters = context.Characters.Where(x => enemyEntry.Characters.Contains(x.Name)).ToList();
                    //if (tempCharacters.Count() != enemyEntry.Characters.Count())
                    // Console.WriteLine();

                    var tempWorlds = context.Worlds.Where(x => enemyEntry.Worlds.Contains(x.Name)).ToList();
                    if (tempWorlds.Count() != enemyEntry.Worlds.Count())
                        Console.WriteLine();

                    context.JournalEntries.Add(new JournalEntry
                    {
                        Title = enemyEntry.Title,
                        Characters = tempCharacters,
                        Worlds = tempWorlds,
                        Description = enemyEntry.Description,
                        AdditionalInformation = enemyEntry.AdditionalInformation,
                        Game = context.Games.FirstOrDefault(x => x.Name == gameName) ?? new Game(),
                        Category = "Enemy"
                    });
                }
            }

            context.SaveChanges();
        }

        public static void CreateReportEntries(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/jj/_jj_reports.json"));
            var allJJReportEntries = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<JJCharacterObject>>>>(streamReader.ReadToEnd());

            if (!allJJReportEntries!.ContainsKey("Reports"))
                throw new Exception("No Report List Found!");

            foreach (var (gameName, reportEntries) in allJJReportEntries["Reports"])
            {
                if (gameName == "Kingdom Hearts") continue;

                foreach (var enemyEntry in reportEntries)
                {
                    var tempCharacters = context.Characters.Where(x => enemyEntry.Characters.Contains(x.Name)).ToList();
                    if (tempCharacters.Count() != enemyEntry.Characters.Count())
                        Console.WriteLine();

                    var tempWorlds = context.Worlds.Where(x => enemyEntry.Worlds.Contains(x.Name)).ToList();
                    if (tempWorlds.Count() != enemyEntry.Worlds.Count())
                        Console.WriteLine();

                    context.JournalEntries.Add(new JournalEntry
                    {
                        Title = enemyEntry.Title,
                        Characters = tempCharacters,
                        Worlds = tempWorlds,
                        Description = enemyEntry.Description,
                        AdditionalInformation = enemyEntry.AdditionalInformation,
                        Game = context.Games.FirstOrDefault(x => x.Name == gameName) ?? new Game(),
                        Category = "Report"
                    });
                }
            }

            context.SaveChanges();
        }
        #endregion Jiminy's Journal

        #region Moogle Shop

        public class MSInventoryObject
        {
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string AdditionalInformation { get; set; } = string.Empty;
            public int Cost { get; set; } = 0;
            public string Currency { get; set; } = string.Empty;
            public List<MSEnemyDropObject> EnemyDrops { get; set; } = new();
        }

        public class MSEnemyDropObject
        {
            public string EnemyName { get; set; } = string.Empty;
            public float DropRate { get; set; } = 0.0f;
            public string AdditionalInformation { get; set; } = string.Empty;
        }
        public static void CreateInventory(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/ms/_ms_inventory.json"));
            var allMSInventory = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<MSInventoryObject>>>>(streamReader.ReadToEnd());

            if (!allMSInventory!.ContainsKey("Inventory"))
                throw new Exception("No Inventory List Found!");

            foreach (var (gameName, inventory) in allMSInventory["Inventory"])
            {
                if (gameName != "Kingdom Hearts Melody of Memory") continue;

                foreach (var item in inventory)
                {
                    var tempItem = context.Inventory.FirstOrDefault(x => x.Name == item.Name && x.Game.Name == gameName);
                    if (tempItem != null)
                        continue;

                    var drops = new List<EnemyDrop>();
                    foreach (var drop in item.EnemyDrops)
                    {
                        var tempCharacter = context.Characters.AsTracking().Include(x => x.CharacterLocations).Where(x => x.Name.Contains(drop.EnemyName)).FirstOrDefault();
                        if (tempCharacter == null)
                        {
                            Console.WriteLine();
                            continue;
                        }

                        foreach (var characterLocation in tempCharacter.CharacterLocations)
                        {
                            var tempDrop = new EnemyDrop
                            {
                                DropRate = drop.DropRate,
                                AdditionalInformation = drop.AdditionalInformation,
                                //Inventory = tempItem,
                                CharacterLocation = characterLocation
                            };

                            drops.Add(tempDrop);
                        }
                    }

                    //tempItem.EnemyDrops = drops;

                    context.Inventory.Add(new Inventory
                    {
                        Name = item.Name,
                        Category = item.Category,
                        Description = item.Description,
                        AdditionalInformation = item.AdditionalInformation,
                        Cost = item.Cost,
                        Currency = item.Currency,
                        Game = context.Games.FirstOrDefault(x => x.Name == gameName) ?? new Game(),
                        EnemyDrops = drops
                    });
                }
            }

            context.SaveChanges();
        }

        public class MSRecipeObject
        {
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string UnlockConditionDescription { get; set; } = string.Empty;
            public Dictionary<string, int> MaterialsNeeded { get; set; } = new();
        }
        public static void CreateRecipes(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/ms/_ms_recipes.json"));
            var allMSRecipes = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<MSRecipeObject>>>>(streamReader.ReadToEnd());

            if (!allMSRecipes!.ContainsKey("Recipes"))
                throw new Exception("No Recipe List Found!");

            foreach (var (gameName, recipes) in allMSRecipes["Recipes"])
            {
                if (gameName != "Kingdom Hearts Melody of Memory") continue;

                foreach (var recipe in recipes)
                {
                    var tempRecipe = context.Recipes.FirstOrDefault(x => x.Name == recipe.Name && x.Game.Name == gameName);
                    if (tempRecipe != null)
                        continue;

                    var recipeMaterials = new List<RecipeMaterial>();
                    foreach (var (name, amount) in recipe.MaterialsNeeded)
                    {
                        var tempItem = context.Inventory.FirstOrDefault(x => name == x.Name && gameName == x.Game.Name);
                        if (tempItem == null)
                        {
                            Console.WriteLine();
                            continue;
                        }

                        recipeMaterials.Add(new RecipeMaterial
                        {
                            Inventory = tempItem,
                            Amount = amount
                        });
                    }

                    context.Recipes.Add(new Recipe
                    {
                        Name = recipe.Name,
                        Category = recipe.Category,
                        UnlockConditionDescription = recipe.UnlockConditionDescription,
                        Game = context.Games.FirstOrDefault(x => x.Name == gameName) ?? new Game(),
                        RecipeMaterials = recipeMaterials
                    });
                }
            }

            context.SaveChanges();
        }
        #endregion Moogle Shop

        #region Misc

        public class MSMiscEnemyObject
        {
            public string CharacterName { get; set; } = string.Empty;
            public Dictionary<string, List<string>> WorldWithAreas { get; set; } = new Dictionary<string, List<string>>();
        }
        public static void CreateEnemyLocations(WayfinderContext context)
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/seed/_misc/enemy_locations/_kh1_enemy_locations.json"));
            var allMSEnemyLocations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<MSMiscEnemyObject>>>>(streamReader.ReadToEnd());

            if (!allMSEnemyLocations!.ContainsKey("EnemyLocations"))
                throw new Exception("No Enemy Location List Found!");

            foreach (var (gameName, locations) in allMSEnemyLocations["EnemyLocations"])
            {
                if (gameName != "Kingdom Hearts") continue;

                foreach (var location in locations)
                {
                    var character = context.Characters.FirstOrDefault(x => x.Name == location.CharacterName);
                    if (character == null)
                    {
                        Console.WriteLine();
                        continue;
                    }

                    var tempLocation = context.CharacterLocations.FirstOrDefault(x => x.Character.Id == character.Id && x.Game.Name == gameName);
                    if (tempLocation != null)
                        continue;

                    var characterLocations = new List<CharacterLocation>();
                    foreach (var (worldName, areaNames) in location.WorldWithAreas)
                    {
                        var world = context.Worlds.First(x => worldName == x.Name);
                        if (world == null)
                            Console.WriteLine();

                        var areas = context.Areas.Where(x => areaNames.Contains(x.Name));

                        var characterLocation = new CharacterLocation
                        {
                            Character = character,
                            Game = context.Games.FirstOrDefault(x => x.Name == gameName) ?? new Game(),
                            World = world,
                            Areas = areas.ToList()
                        };

                        characterLocations.Add(characterLocation);
                    }

                    character.CharacterLocations = characterLocations;
                }
            }

            context.SaveChanges();
        }
        #endregion Misc
    }
}
