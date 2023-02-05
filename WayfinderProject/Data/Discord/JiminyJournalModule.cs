﻿using Discord;
using Discord.Commands;
using System.Web;
using WayfinderProjectAPI.Controllers;
using WayfinderProjectAPI.Data;

namespace WayfinderProject.Data.Discord
{
    [Group("jj")]
    public class JiminyJournalModule : ModuleBase<SocketCommandContext>
    {
        private IServiceProvider serviceProvider;

        public JiminyJournalModule(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [Command("help")]
        [Summary("Returns all commands, how to use them and what they provide.")]
        public async Task HelpAsync()
        {
            try
            {
                await Context.Channel.SendMessageAsync(
                        $"```\r\n" +
                        $"!jj daily - Returns the daily entry.\r\n" +
                        $"!jj random_entry - Returns a random entry result.\r\n" +
                        $"!jj search_entry <search_criteria> - Returns up to 25 search results that meet your criteria. If your search criteria contains any spaces, wrap your query in two quotes, i.e. \"Sora III\".\r\n" +
                        $"!jj entry <entry_id> - Returns a entry with the specific entry id. Find an id either through random or search.\r\n" +
                        $"```");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("daily")]
        [Summary("Returns a daily entry.")]
        public async Task DailyEntryAsync()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var dailyEntry = await api.GetDailyEntry();
                    await Context.Channel.SendMessageAsync(
                        $"**Entry Name:** {dailyEntry.Title} (Id: {dailyEntry.Id})\r\n" +
                        $"**Game:** {dailyEntry.Game.Name}\r\n" +
                        $"**Category:** {dailyEntry.Category}\r\n" +
                        $"**Description:** {dailyEntry.Description}\r\n" +
                        $"**Additional Information:** {dailyEntry.AdditionalInformation}\r\n" +
                        $"\r\nFind more information at https://wayfinderprojectkh.com/jiminy_journal?category={HttpUtility.UrlEncode(dailyEntry.Category)}&entry={dailyEntry.Id}&open_row={dailyEntry.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("random_entry")]
        [Summary("Returns a random entry.")]
        public async Task RandomEntryAsync()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var randomEntry = await api.GetRandomEntry();
                    await Context.Channel.SendMessageAsync(
                        $"**Entry Name:** {randomEntry.Title} (Id: {randomEntry.Id})\r\n" +
                        $"**Game:** {randomEntry.Game.Name}\r\n" +
                        $"**Category:** {randomEntry.Category}\r\n" +
                        $"**Description:** {randomEntry.Description}\r\n" +
                        $"**Additional Information:** {randomEntry.AdditionalInformation}\r\n" +
                        $"\r\nFind more information at https://wayfinderprojectkh.com/jiminy_journal?category={HttpUtility.UrlEncode(randomEntry.Category)}&entry={randomEntry.Id}&open_row={randomEntry.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("search_entry")]
        [Summary("Returns entries with search criteria. If your search criteria contains spaces, wrap in quotes.")]
        public async Task SearchEntriesAsync(string criteria)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var message = await Context.Channel.SendMessageAsync("Searching...");

                    var entries = await api.GetJournalEntries("bfb7c747-6df4-40b5-9015-e36be500ab45", "", "", "", "", criteria);

                    var embedBuilder = new EmbedBuilder
                    {
                        Title = "Entry Search Results",
                        Url = $"https://wayfinderprojectkh.com/memory_archive?text={HttpUtility.UrlEncode(criteria)}",
                        Description = "Use the Ids of the search results to lookup the entries you are looking for, for example **!jj entry <entry_id>**. Note: Limited to 25 results."
                    };

                    foreach (var scene in entries.Take(25))
                    {
                        embedBuilder.AddField(new EmbedFieldBuilder
                        {
                            Name = scene.Id.ToString(),
                            Value = scene.Title
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

        [Command("entry")]
        [Summary("Returns entry by id.")]
        public async Task GetEntryAsync(int id)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();
                    var api = new WayfinderController(context);

                    var entry = await api.GetEntry(id);
                    await Context.Channel.SendMessageAsync(
                        $"**Entry Name:** {entry.Title} (Id: {entry.Id})\r\n" +
                        $"**Game:** {entry.Game.Name}\r\n" +
                        $"**Category:** {entry.Category}\r\n" +
                        $"**Description:** {entry.Description}\r\n" +
                        $"**Additional Information:** {entry.AdditionalInformation}\r\n" +
                        $"\r\nFind more information at https://wayfinderprojectkh.com/jiminy_journal?category={entry.Category}&entry={entry.Id}&open_row={entry.Id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}