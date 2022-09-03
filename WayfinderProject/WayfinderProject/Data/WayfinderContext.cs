using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Data
{
    public class WayfinderContext : DbContext
    {
        public WayfinderContext(DbContextOptions<WayfinderContext> options) : base(options)
        {
            //this.Initialize();
        }

        public void Initialize()
        {
            this.Areas.RemoveRange(this.Areas);
            this.Characters.RemoveRange(this.Characters);
            this.Games.RemoveRange(this.Games);
            this.Music.RemoveRange(this.Music);
            this.Scenes.RemoveRange(this.Scenes);
            this.Worlds.RemoveRange(this.Worlds);
            this.Script.RemoveRange(this.Script);

            this.SaveChanges();

            this.Create();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Area>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Character>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Game>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Music>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Scene>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<World>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Script>().Property(p => p.Id).ValueGeneratedOnAdd();
        }

        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Character> Characters { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Music> Music { get; set; } = null!;
        public DbSet<Scene> Scenes { get; set; } = null!;
        public DbSet<World> Worlds { get; set; } = null!;
        public DbSet<Script> Script { get; set; } = null!;


        public void Create()
        {
            // Load Areas Data into Database
            this.CreateAreas();

            // Load Character Data into Database
            this.CreateCharacters();

            // Load Game Data into Database
            this.CreateGames();
            
            // Load Music Data into Database
            this.CreateMusic();
            
            // Load World Data into Database
            this.CreateWorlds();

            // Load Script Data into Database
            this.CreateKH1Script();

            // Load Scene Data into Database
            this.CreateScenes();
        }

        public void CreateAreas()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"Properties/_areas.json"));
            var areas = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(streamReader.ReadToEnd());

            if (!areas!.ContainsKey("Areas"))
                throw new Exception("No Areas List Found!");

            foreach (var area in areas["Areas"])
                this.Areas.Add(new Area { Name = area });

            this.SaveChanges();
        }

        public void CreateCharacters()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"Properties/_characters.json"));
            var characters = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(streamReader.ReadToEnd());

            if (!characters!.ContainsKey("Characters"))
                throw new Exception("No Characters List Found!");

            foreach (var character in characters["Characters"])
                this.Characters.Add(new Character { Name = character });

            this.SaveChanges();
        }

        public void CreateGames()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"Properties/_games.json"));
            var games = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(streamReader.ReadToEnd());

            if (!games!.ContainsKey("Games"))
                throw new Exception("No Games List Found!");

            foreach (var game in games["Games"])
                this.Games.Add(new Game { Name = game });

            this.SaveChanges();
        }

        class MusicObject
        {
            public string Name { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;
        }

        public void CreateMusic()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"Properties/_music.json"));
            var music = JsonSerializer.Deserialize<Dictionary<string, List<MusicObject>>>(streamReader.ReadToEnd());

            if (!music!.ContainsKey("Music"))
                throw new Exception("No Music List Found!");

            foreach (var song in music["Music"])
                this.Music.Add(new Music { Name = song.Name, Link = song.Link });

            this.SaveChanges();
        }

        public void CreateWorlds()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"Properties/_worlds.json"));
            var worlds = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(streamReader.ReadToEnd());

            if (!worlds!.ContainsKey("Worlds"))
                throw new Exception("No Worlds List Found!");

            foreach (var world in worlds["Worlds"])
                this.Worlds.Add(new World { Name = world });

            this.SaveChanges();
        }

        class LineScriptObject
        {
            public int Order { get; set; }
            public string Character { get; set; } = string.Empty;
            public string Line { get; set; } = string.Empty;
        }

        public void CreateKH1Script()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"Properties/_kh1_lines.json"));
            var script = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<LineScriptObject>>>>(streamReader.ReadToEnd());

            if (!script!.ContainsKey("Script"))
                throw new Exception("No Script List Found!");

            foreach (var (scene, lines) in script["Script"])
            {
                var temp = new Script { SceneName = scene, GameName = "Kingdom Hearts", Lines = new List<ScriptLine>() };
                foreach (var line in lines)
                {
                    temp.Lines.Add(new ScriptLine { Order = line.Order, Character = line.Character, Line = line.Line });
                }
                
                this.Script.Add(temp);
            }

            this.SaveChanges();
        }

        class SceneObject
        {
            public string Name { get; set; }
            public string Link { get; set; }
            public List<string> Worlds { get; set; }
            public List<string> Characters { get; set; }
            public List<string> Areas { get; set; }
            public List<string> Music { get; set; }
        }

        public void CreateScenes()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"Properties/_scenes.json"));
            var allScenes = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<SceneObject>>>>(streamReader.ReadToEnd());

            if (!allScenes!.ContainsKey("Scenes"))
                throw new Exception("No Scenes List Found!");

            foreach (var (gameName, scenes) in allScenes["Scenes"])
            {
                foreach (var scene in scenes)
                {
                    var game = this.Games.FirstOrDefault(x => x.Name == gameName);
                    var worlds = this.Worlds.Where(x => scene.Worlds.Contains(x.Name)).ToList();
                    var characters = this.Characters.Where(x => scene.Characters.Contains(x.Name)).ToList();
                    var areas = this.Areas.Where(x => scene.Areas.Contains(x.Name)).ToList();
                    var music = this.Music.Where(x => scene.Music.Contains(x.Name)).ToList();
                    var script = this.Script.FirstOrDefault(x => x.SceneName == scene.Name && x.GameName == gameName) ??
                        this.Script.FirstOrDefault(x => x.SceneName == "None");

                    this.Scenes.Add(new Scene
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

            this.SaveChanges();
        }
    }
}