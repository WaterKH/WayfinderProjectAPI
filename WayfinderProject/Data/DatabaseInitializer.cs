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

            // Load Areas Data into Database
            //CreateAreas(context);

            //// Load Character Data into Database
            //CreateCharacters(context);

            //// Load Game Data into Database
            //CreateGames(context);

            //// Load Music Data into Database
            //CreateMusic(context);

            //// Load World Data into Database
            //CreateWorlds(context);

            //// Load Script Data into Database
            //CreateScripts(context);

            //// Load Scene Data into Database
            //CreateScenes(context);

            // JJ
            // Character
            //CreateCharacterEntries(context);

            // Story
            //CreateStoryEntries(context);

            // Enemy
            //CreateEnemiesEntries(context);

            // Report
            //CreateReportEntries(context);

            IsInitializing = false;
        }

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
                if (gameName != "Kingdom Hearts") continue;

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
                if (gameName != "Kingdom Hearts") continue;

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
                if (gameName != "Kingdom Hearts") continue;

                foreach (var enemyEntry in enemyEntries)
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
                if (gameName != "Kingdom Hearts") continue;

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
    }
}
