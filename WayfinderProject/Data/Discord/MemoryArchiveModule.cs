using Discord;
using Discord.Commands;
using System.Web;
using WayfinderProjectAPI.Controllers;
using WayfinderProjectAPI.Data;

namespace WayfinderProject.Data.Discord
{
    [Group("ma")]
    public class MemoryArchiveModule : ModuleBase<SocketCommandContext>
    {
        private IServiceProvider serviceProvider;

        public MemoryArchiveModule(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [Command("random_scene")]
        [Summary("Returns a random scene.")]
        public async Task RandomSceneAsync()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var randomScene = await api.GetRandomScene();
                    await Context.Channel.SendMessageAsync(
                        $"**Scene Name:** {randomScene.Name} (Id: {randomScene.Id})\r\n" +
                        $"**Game:** {randomScene.Game.Name}\r\n" +
                        $"**Characters:** {string.Join(", ", randomScene.Characters.Select(x => x.Name))}\r\n" +
                        $"**Worlds:** {string.Join(", ", randomScene.Worlds.Select(x => x.Name))}\r\n" +
                        $"**Music:** {string.Join(", ", randomScene.Music.Select(x => x.Name))}\r\n" +
                        $"**Link:** {randomScene.Link}" +
                        $"\r\n\r\nFind more information at https://wayfinderprojectkh.com/memory_archive?category=Scenes&scene={randomScene.Id}&open_row={randomScene.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("search_scene")]
        [Summary("Returns scenes with search criteria.")]
        public async Task SearchSceneAsync(string criteria)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var message = await Context.Channel.SendMessageAsync("Searching...");

                    var scenes = await api.GetScenes("bfb7c747-6df4-40b5-9015-e36be500ab45", "", "", "", "", "", "", criteria);

                    var embedBuilder = new EmbedBuilder
                    {
                        Title = "Scene Search Results",
                        Url = $"https://wayfinderprojectkh.com/memory_archive?category=Scenes&text={HttpUtility.UrlEncode(criteria)}",
                        Description = "Use the Ids of the search results to lookup the scene you are looking for, for example **!ma scene <scene_id>**. Note: Limited to 25 results."
                    };

                    foreach (var scene in scenes.Take(25))
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

        [Command("scene")]
        [Summary("Returns scene by id.")]
        public async Task GetSceneAsync(int id)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var scene = await api.GetScene(id);
                    await Context.Channel.SendMessageAsync(
                        $"**Scene Name:** {scene.Name} (Id: {scene.Id})\r\n" +
                        $"**Game:** {scene.Game.Name}\r\n" +
                        $"**Characters:** {string.Join(", ", scene.Characters.Select(x => x.Name))}\r\n" +
                        $"**Worlds:** {string.Join(", ", scene.Worlds.Select(x => x.Name))}\r\n" +
                        $"**Music:** {string.Join(", ", scene.Music.Select(x => x.Name))}\r\n" +
                        $"**Link:** {scene.Link}" +
                        $"\r\n\r\nFind more information at https://wayfinderprojectkh.com/memory_archive?category=Scenes&scene={scene.Id}&open_row={scene.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("random_interview")]
        [Summary("Returns a random interview.")]
        public async Task RandomInterviewAsync()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var randomInterview = await api.GetRandomInterview();
                    await Context.Channel.SendMessageAsync(
                        $"**Interview Name:** {randomInterview.Name} (Id: {randomInterview.Id})\r\n" +
                        $"**Games:** {string.Join(", ", randomInterview.Games.Select(x => x.Name))}\r\n" +
                        $"**Participants:** {string.Join(", ", randomInterview.Participants.Select(x => x.Name))}\r\n" +
                        $"**Translator:** {randomInterview.Translator.Name}\r\n" +
                        $"**Provider:** {randomInterview.Provider.Name}\r\n" +
                        $"**Link:** {randomInterview.Link}" +
                        $"\r\n\r\nFind more information at https://wayfinderprojectkh.com/memory_archive?category=Interviews&scene={randomInterview.Id}&open_row={randomInterview.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("search_interview")]
        [Summary("Returns interviews with search criteria.")]
        public async Task SearchInterviewAsync(string criteria)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var message = await Context.Channel.SendMessageAsync("Searching...");

                    var interviews = await api.GetInterviews("bfb7c747-6df4-40b5-9015-e36be500ab45", "", "", "", "", "", criteria);

                    var embedBuilder = new EmbedBuilder
                    {
                        Title = "Interview Search Results",
                        Url = $"https://wayfinderprojectkh.com/memory_archive?category=Interviews&text={HttpUtility.UrlEncode(criteria)}",
                        Description = "Use the Ids of the search results to lookup the interview you are looking for, for example **!ma interview <interview_id>**. Note: Limited to 25 results."
                    };

                    foreach (var scene in interviews.Take(25))
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

        [Command("interview")]
        [Summary("Returns interview by id.")]
        public async Task GetInterviewAsync(int id)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var interview = await api.GetInterview(id);
                    await Context.Channel.SendMessageAsync(
                        $"**Interview Name:** {interview.Name} (Id: {interview.Id})\r\n" +
                        $"**Games:** {string.Join(", ", interview.Games.Select(x => x.Name))}\r\n" +
                        $"**Participants:** {string.Join(", ", interview.Participants.Select(x => x.Name))}\r\n" +
                        $"**Translator:** {interview.Translator.Name}\r\n" +
                        $"**Provider:** {interview.Provider.Name}\r\n" +
                        $"**Link:** {interview.Link}" +
                        $"\r\n\r\nFind more information at https://wayfinderprojectkh.com/memory_archive?category=Interviews&scene={interview.Id}&open_row={interview.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}