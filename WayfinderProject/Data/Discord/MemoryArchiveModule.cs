using Discord.Commands;
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
                    await Context.Channel.SendMessageAsync($"{randomScene.Link}\r\nFind more information at https://wayfinderprojectkh.com/memory_archive?category=Scenes&scene={randomScene.Id}&open_row={randomScene.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}