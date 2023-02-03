using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WayfinderProjectAPI.Data;
using WayfinderProjectAPI.Data.DTOs;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WayfinderController : ControllerBase
    {
        //private readonly ILogger<WayfinderController> _logger;
        private readonly WayfinderContext _context;

        public WayfinderController(WayfinderContext context)
        {
            //_logger = logger;
            _context = context;
        }

        #region Memory Archive
        [HttpGet("GetScenes")]
        public async Task<List<SceneDto>> GetScenes([FromQuery] string accountId, [FromQuery] string? games = null, [FromQuery] string? scenes = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? areas = null, [FromQuery] string? music = null, [FromQuery] string? line = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null)
            {
                settings = new SearchSettings { AccountId = accountId };

                _context.SearchSettings.Add(settings);
                _context.SaveChanges();
            }

            var results = _context.Scenes.AsNoTrackingWithIdentityResolution();

            // See if we search for favourites

            var queryString = (line == null ? line : line.Any(x => char.IsPunctuation(x)) ? line : Regex.Replace(line, @"[^\w\s]", ""));

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (scenes != null)
            {
                var scenesList = scenes.Split("::").Select(x => x.Trim());

                results = results.Where(x => scenesList.Contains(x.Name));
            }

            if (worlds != null)
            {
                var worldsList = worlds.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Worlds).Where(x => x.Worlds.Any(y => worldsList.Contains(y.Name))))
                {
                    if (result == null || result.Worlds == null) continue;

                    if (worldsList.All(x => result.Worlds.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (areas != null)
            {
                var areasList = areas.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Areas).Where(x => x.Areas.Any(y => areasList.Contains(y.Name))))
                {
                    if (result == null || result.Areas == null) continue;

                    if (areasList.All(x => result.Areas.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (characters != null)
            {
                var charactersList = characters.Split("::").Select(x => x.Trim());

                // Create Aliases
                Dictionary<string, List<string>> aliases = new Dictionary<string, List<string>>();
                List<string> aliasAppearAs = new List<string>();

                if (settings.IncludeAlias)
                {
                    await this._context.Aliases.Where(x => charactersList.Contains(x.Original)).ForEachAsync(x =>
                    {
                        if (!aliases.ContainsKey(x.Original))
                        {
                            aliases.Add(x.Original, new List<string>());
                        }

                        aliases[x.Original].Add(x.AppearAs);
                        aliasAppearAs.Add(x.AppearAs);
                    });
                }

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Characters).Where(x => x.Characters.Any(y => charactersList.Contains(y.Name) || aliasAppearAs.Contains(y.Name))))
                {
                    if (result == null || result.Characters == null) continue;

                    if (charactersList.All(x => result.Characters.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }

                    // Look up aliases of characters
                    if (settings.IncludeAlias)
                    {
                        foreach (var (original, appearAs) in aliases)
                        {
                            foreach (var character in appearAs)
                            {
                                var newCharactersList = charactersList.ToList();
                                newCharactersList.Remove(original);
                                newCharactersList.Add(character);

                                if (newCharactersList.All(x => result.Characters.Select(y => y.Name).Any(y => y == x)))
                                {
                                    resultIds.Add(result.Id);
                                }
                            }
                        }
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (music != null)
            {
                var musicList = music.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Music).Where(x => x.Music.Any(y => musicList.Contains(y.Name))))
                {
                    if (result == null || result.Music == null) continue;

                    if (musicList.All(x => result.Music.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (queryString != null)
            {
                if (!queryString.Contains("\""))
                {
                    queryString = queryString.ToLower();

                    var tempResults = new List<Scene>();
                    foreach (var result in results.Include(x => x.Script).Include(x => x.Script.Lines))
                    {
                        if (result.Script.Lines == null)
                            continue;

                        foreach (var scriptLine in result.Script.Lines)
                        {
                            var tempLine = scriptLine.Line;
                            if (!queryString.Any(x => char.IsPunctuation(x)))
                                tempLine = Regex.Replace(scriptLine.Line, @"[^\w\s]", "");

                            if (tempLine.ToLower().Contains(queryString))
                                tempResults.Add(result);
                        }
                    }

                    results = results.Where(x => tempResults.Select(y => y.Id).Contains(x.Id));
                }
                else
                {
                    queryString = queryString.Replace("\"", "");

                    results = results.Where(x => x.Script.Lines.ToList().Any(y => y.Line.Contains(queryString)));
                }

                if (settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryScenes(queryString, results);
                }
            }

            // Add to search history
            if (settings.TrackHistory)
            {
                this.InsertSearchHistory(accountId, "Memory Archive", "Scenes",
                    queryString ?? string.Empty, scenes ?? string.Empty,
                    games ?? string.Empty, worlds ?? string.Empty, areas ?? string.Empty, characters ?? string.Empty, music ?? string.Empty);
            }

            // Check for Favourites and Projects
            results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }

        private IQueryable<Scene> MainSearchQueryScenes(string queryString, IQueryable<Scene> scenes)
        {
            // Combine our incoming scenes with what we find with the straight query search
            var results = _context.Scenes.AsNoTrackingWithIdentityResolution();

            var resultIds = results
                .Where(x =>
                    x.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Game.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Areas.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Characters.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Music.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Worlds.Any(y => y.Name.ToLower().Contains(queryString.ToLower()))
                ).Select(x => x.Id).ToList();

            var allIds = scenes.Select(x => x.Id).ToList().Union(resultIds);

            return results.Where(x => allIds.Contains(x.Id));
        }

        private IQueryable<Scene> GetFavouritesProjectsSearchResults(string accountId, SearchSettings settings, IQueryable<Scene> results)
        {
            if (settings.FavouriteSearch && settings.ProjectSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Memory Archive" && x.Category == "Scenes").Select(x => x.SpecificRecordId);
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Memory Archive" && x.Category == "Scenes").Select(x => x.SpecificRecordId);

                var combinedIds = favouriteIds.Union(projectRecordIds);

                results = results.Where(x => combinedIds.Contains(x.Id));
            }
            else if (settings.FavouriteSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Memory Archive" && x.Category == "Scenes").Select(x => x.SpecificRecordId);

                results = results.Where(x => favouriteIds.Contains(x.Id));
            }
            else if (settings.ProjectSearch)
            {
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Memory Archive" && x.Category == "Scenes").Select(x => x.SpecificRecordId);

                results = results.Where(x => projectRecordIds.Contains(x.Id));
            }

            return results;
        }

        [HttpGet("GetScript")]
        public async Task<ScriptDto> GetScript([FromQuery] string gameName, [FromQuery] string sceneName)
        {
            var result = await _context.Script.AsNoTrackingWithIdentityResolution().Include(x => x.Lines).FirstOrDefaultAsync(x => x.GameName == gameName && x.SceneName == sceneName);

            if (result == null)
            {
                return new ScriptDto
                {
                    GameName = gameName,
                    SceneName = sceneName,
                    Lines = gameName == "Kingdom Hearts Union χ" ?
                        new List<ScriptLineDto> { new ScriptLineDto { Character = "None", Line = "This will be updated after Damo279's big project." } } :
                        new List<ScriptLineDto> { new ScriptLineDto { Character = "None", Line = "None" } }
                };
            }

            return result.ToDto();
        }

        [HttpGet("GetInterviews")]
        public async Task<List<InterviewDto>> GetInterviews([FromQuery] string accountId, [FromQuery] string? interviews = null, [FromQuery] string? games = null, [FromQuery] string? participants = null, [FromQuery] string? providers = null, [FromQuery] string? translators = null, [FromQuery] string? line = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null)
            {
                settings = new SearchSettings { AccountId = accountId };

                _context.SearchSettings.Add(settings);
                _context.SaveChanges();
            }

            var results = _context.Interviews.AsNoTrackingWithIdentityResolution();
            var queryString = (line == null ? line : line.Any(x => char.IsPunctuation(x)) ? line : Regex.Replace(line, @"[^\w\s]", ""));

            if (interviews != null)
            {
                var scenesList = interviews.Split("::").Select(x => x.Trim());

                results = results.Where(x => scenesList.Contains(x.Name));
            }

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Games).Where(x => x.Games.Any(y => gamesList.Contains(y.Name))))
                {
                    if (result == null || result.Games == null) continue;

                    if (gamesList.All(x => result.Games.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (participants != null)
            {
                var participantsList = participants.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Participants).Where(x => x.Participants.Any(y => participantsList.Contains(y.Name))))
                {
                    if (result == null || result.Participants == null) continue;

                    if (participantsList.All(x => result.Participants.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (providers != null)
            {
                var providersList = providers.Split("::").Select(x => x.Trim());

                results = results.Where(x => providersList.Contains(x.Provider.Name));
            }

            if (translators != null)
            {
                var translatorsList = translators.Split("::").Select(x => x.Trim());

                results = results.Where(x => translatorsList.Contains(x.Translator.Name));
            }

            if (queryString != null)
            {
                if (!queryString.Contains("\""))
                {
                    queryString = queryString.ToLower();

                    var tempResults = new List<Interview>();
                    foreach (var result in results.Include(x => x.Conversation))
                    {
                        if (result.Conversation == null)
                            continue;

                        foreach (var conversationLine in result.Conversation)
                        {
                            var tempLine = conversationLine.Line;
                            if (!queryString.Any(x => char.IsPunctuation(x)))
                                tempLine = Regex.Replace(conversationLine.Line, @"[^\w\s]", "");

                            if (tempLine.ToLower().Contains(queryString))
                                tempResults.Add(result);
                        }
                    }

                    results = results.Where(x => tempResults.Select(y => y.Id).Contains(x.Id));
                }
                else
                {
                    queryString = queryString.Replace("\"", "");

                    results = results.Where(x => x.Conversation.ToList().Any(y => y.Line.Contains(queryString)));
                }

                if (settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryInterviews(queryString, results);
                }
            }

            // Add to search history
            if (settings.TrackHistory)
            {
                this.InsertSearchHistory(accountId, "Memory Archive", "Interviews",
                    queryString ?? string.Empty, interviews ?? string.Empty,
                    games ?? string.Empty, participants ?? string.Empty, providers ?? string.Empty, translators ?? string.Empty);
            }

            // Check for Favourites and Projects
            results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }

        private IQueryable<Interview> MainSearchQueryInterviews(string queryString, IQueryable<Interview> interviews)
        {
            // Combine our incoming scenes with what we find with the straight query search
            var results = _context.Interviews.AsNoTrackingWithIdentityResolution();

            var resultIds = results
                .Where(x =>
                    x.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Games.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Participants.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Provider.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Translator.Name.ToLower().Contains(queryString.ToLower())
                ).Select(x => x.Id).ToList();

            var allIds = interviews.Select(x => x.Id).ToList().Union(resultIds);

            return results.Where(x => allIds.Contains(x.Id));
        }

        private IQueryable<Interview> GetFavouritesProjectsSearchResults(string accountId, SearchSettings settings, IQueryable<Interview> results)
        {
            if (settings.FavouriteSearch && settings.ProjectSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Memory Archive" && x.Category == "Interviews").Select(x => x.SpecificRecordId);
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Memory Archive" && x.Category == "Interviews").Select(x => x.SpecificRecordId);

                var combinedIds = favouriteIds.Union(projectRecordIds);

                results = results.Where(x => combinedIds.Contains(x.Id));
            }
            else if (settings.FavouriteSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Memory Archive" && x.Category == "Interviews").Select(x => x.SpecificRecordId);

                results = results.Where(x => favouriteIds.Contains(x.Id));
            }
            else if (settings.ProjectSearch)
            {
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Memory Archive" && x.Category == "Interviews").Select(x => x.SpecificRecordId);

                results = results.Where(x => projectRecordIds.Contains(x.Id));
            }

            return results;
        }

        [HttpGet("GetInterviewConversation")]
        public async Task<List<InterviewLineDto>> GetInterviewConversation([FromQuery] string name, [FromQuery] string providerName, [FromQuery] string translatorName)
        {
            var result = await _context.Interviews.AsNoTrackingWithIdentityResolution().Include(x => x.Conversation).Where(x => x.Name == name && x.Provider.Name == providerName && x.Translator.Name == translatorName).SelectMany(x => x.Conversation).ToListAsync();

            if (result == null)
            {
                return new List<InterviewLineDto>
                {
                    new InterviewLineDto { Line = "Unavailable at this time.", Speaker = "None" }
                };
            }

            return result.ToDto().ToList();
        }
        #endregion Memory Archive

        #region Jiminy Journal
        [HttpGet("SearchForJournalEntries")]
        public async Task<List<JournalEntryDto>> SearchForJournalEntries([FromQuery] string accountId, [FromQuery] string? games = null, [FromQuery] string? entries = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? information = null, [FromQuery] string? category = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null)
            {
                settings = new SearchSettings { AccountId = accountId };

                _context.SearchSettings.Add(settings);
                _context.SaveChanges();
            }

            var results = _context.JournalEntries.AsNoTrackingWithIdentityResolution().Where(x => x.Category == category);
            var queryString = (information == null ? information : information.Any(x => char.IsPunctuation(x)) ? information : Regex.Replace(information, @"[^\w\s]", ""));

            if (entries != null)
            {
                var titlesList = entries.Split("::").Select(x => x.Trim());

                results = results.Where(x => titlesList.Contains(x.Title));
            }

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (characters != null)
            {
                var charactersList = characters.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Characters).Where(x => x.Characters.Any(y => charactersList.Contains(y.Name))))
                {
                    if (result == null || result.Characters == null) continue;

                    if (charactersList.All(x => result.Characters.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (worlds != null)
            {
                var worldsList = worlds.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Worlds).Where(x => x.Worlds.Any(y => worldsList.Contains(y.Name))))
                {
                    if (result == null || result.Worlds == null) continue;

                    if (worldsList.All(x => result.Worlds.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (queryString != null)
            {
                if (!queryString.Contains("\""))
                {
                    queryString = queryString.ToLower();

                    var tempResults = new List<JournalEntry>();
                    foreach (var result in results)
                    {
                        // Description search
                        var tempDescription = result.Description;
                        if (!queryString.Any(x => char.IsPunctuation(x)))
                            tempDescription = Regex.Replace(result.Description, @"[^\w\s]", "");

                        if (tempDescription.ToLower().Contains(queryString))
                            tempResults.Add(result);

                        // Additional Information search
                        var tempAdditionalInformation = result.AdditionalInformation;
                        if (!queryString.Any(x => char.IsPunctuation(x)))
                            tempAdditionalInformation = Regex.Replace(result.AdditionalInformation, @"[^\w\s]", "");

                        if (tempAdditionalInformation.ToLower().Contains(queryString))
                            tempResults.Add(result);
                    }

                    results = results.Where(x => tempResults.Select(y => y.Id).Contains(x.Id));
                }
                else
                {
                    queryString = queryString.Replace("\"", "");

                    results = results.Where(x => x.Description.Contains(queryString) || x.AdditionalInformation.Contains(queryString));
                }

                if (settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryJournalEntries(queryString, results);
                }
            }

            // Add to search history
            if (settings.TrackHistory)
            {
                this.InsertSearchHistory(accountId, "Jiminy's Journal", category ?? "Journal Entry",
                    queryString ?? string.Empty, entries ?? string.Empty,
                    games ?? string.Empty, characters ?? string.Empty, worlds ?? string.Empty);
            }

            // Check for Favourites and Projects
            results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }

        private IQueryable<JournalEntry> MainSearchQueryJournalEntries(string queryString, IQueryable<JournalEntry> entries)
        {
            // Combine our incoming scenes with what we find with the straight query search
            var results = _context.JournalEntries.AsNoTrackingWithIdentityResolution();

            var resultIds = results
                .Where(x =>
                    x.Title.ToLower().Contains(queryString.ToLower()) ||
                    x.Game.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Characters.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Worlds.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Category.ToLower().Contains(queryString.ToLower())
                ).Select(x => x.Id).ToList();

            var allIds = entries.Select(x => x.Id).ToList().Union(resultIds);

            return results.Where(x => allIds.Contains(x.Id));
        }

        private IQueryable<JournalEntry> GetFavouritesProjectsSearchResults(string accountId, SearchSettings settings, IQueryable<JournalEntry> results)
        {
            if (settings.FavouriteSearch && settings.ProjectSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Jiminy's Journal").Select(x => x.SpecificRecordId);
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Jiminy's Journal").Select(x => x.SpecificRecordId);

                var combinedIds = favouriteIds.Union(projectRecordIds);

                results = results.Where(x => combinedIds.Contains(x.Id));
            }
            else if (settings.FavouriteSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Jiminy's Journal").Select(x => x.SpecificRecordId);

                results = results.Where(x => favouriteIds.Contains(x.Id));
            }
            else if (settings.ProjectSearch)
            {
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Jiminy's Journal").Select(x => x.SpecificRecordId);

                results = results.Where(x => projectRecordIds.Contains(x.Id));
            }

            return results;
        }

        [HttpGet("GetCharactersFirstAppearance")]
        public async Task<SceneDto?> GetCharactersFirstAppearance([FromQuery] string characterNames, [FromQuery] string? games = null)
        {
            var results = _context.Scenes.AsNoTrackingWithIdentityResolution();

            if (characterNames != null)
            {
                var charactersList = characterNames.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Characters).Where(x => x.Characters.Any(y => charactersList.Contains(y.Name))))
                {
                    if (result == null || result.Characters == null) continue;

                    if (charactersList.All(x => result.Characters.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (results == null)
            {
                return null;
            }

            var returnResults = await results.OrderBy(x => x.Id).ToDto().ToListAsync();

            return returnResults.FirstOrDefault();
        }


        [HttpGet("GetWorldsFirstAppearance")]
        public async Task<SceneDto?> GetWorldsFirstAppearance([FromQuery] string worldNames, [FromQuery] string? games = null)
        {
            var results = _context.Scenes.AsNoTrackingWithIdentityResolution();

            if (worldNames != null)
            {
                var worldsList = worldNames.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.Worlds).Where(x => x.Worlds.Any(y => worldsList.Contains(y.Name))))
                {
                    if (result == null || result.Worlds == null) continue;

                    if (worldsList.All(x => result.Worlds.Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (results == null)
            {
                return null;
            }

            var returnResults = await results.OrderBy(x => x.Id).ToDto().ToListAsync();

            return returnResults.FirstOrDefault();
        }
        #endregion Jiminy Journal

        #region Moogle Shop
        [HttpGet("GetRecipes")]
        public async Task<List<RecipeDto>> GetRecipes([FromQuery] string accountId, [FromQuery] string? games = null, [FromQuery] string? recipes = null, [FromQuery] string? synthMaterials = null, [FromQuery] string? description = null, [FromQuery] string? categories = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null)
            {
                settings = new SearchSettings { AccountId = accountId };

                _context.SearchSettings.Add(settings);
                _context.SaveChanges();
            }

            var results = _context.Recipes.AsNoTrackingWithIdentityResolution();
            var queryString = (description == null ? description : description.Any(x => char.IsPunctuation(x)) ? description : Regex.Replace(description, @"[^\w\s]", ""));

            if (recipes != null)
            {
                var recipeNamesList = recipes.Split("::").Select(x => x.Trim());

                results = results.Where(x => recipeNamesList.Contains(x.Name));
            }

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (categories != null)
            {
                var categoryList = categories.Split("::").Select(x => x.Trim());

                results = results.Where(x => categoryList.Contains(x.Category));
            }

            if (synthMaterials != null)
            {
                var synthList = synthMaterials.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.RecipeMaterials).Where(x => x.RecipeMaterials.Any(y => synthList.Contains(y.Inventory.Name))))
                {
                    if (result == null || result.RecipeMaterials == null) continue;

                    if (synthList.All(x => result.RecipeMaterials.Select(y => y.Inventory.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (queryString != null)
            {
                if (!queryString.Contains("\""))
                {
                    queryString = queryString.ToLower();

                    var tempResults = new List<Recipe>();
                    foreach (var result in results)
                    {
                        // Unlock Description search
                        var tempUnlockDescription = result.UnlockConditionDescription;
                        if (!queryString.Any(x => char.IsPunctuation(x)))
                            tempUnlockDescription = Regex.Replace(result.UnlockConditionDescription, @"[^\w\s]", "");

                        if (tempUnlockDescription.ToLower().Contains(queryString))
                            tempResults.Add(result);

                        // Inventory Description search
                        foreach (var material in result.RecipeMaterials)
                        {
                            // Description search
                            var tempDescription = material.Inventory.Description;
                            if (!queryString.Any(x => char.IsPunctuation(x)))
                                tempDescription = Regex.Replace(material.Inventory.Description, @"[^\w\s]", "");

                            if (tempDescription.ToLower().Contains(queryString))
                                tempResults.Add(result);

                            // Additional Information search
                            var tempAdditionalInformation = material.Inventory.AdditionalInformation;
                            if (!queryString.Any(x => char.IsPunctuation(x)))
                                tempAdditionalInformation = Regex.Replace(material.Inventory.AdditionalInformation, @"[^\w\s]", "");

                            if (tempAdditionalInformation.ToLower().Contains(queryString))
                                tempResults.Add(result);
                        }
                    }

                    results = results.Where(x => tempResults.Select(y => y.Id).Contains(x.Id));
                }
                else
                {
                    queryString = queryString.Replace("\"", "");

                    results = results.Where(x => x.UnlockConditionDescription.Contains(queryString) || x.RecipeMaterials.Any(x => x.Inventory.Description.Contains(queryString)) || x.RecipeMaterials.Any(x => x.Inventory.AdditionalInformation.Contains(queryString)));
                }

                if (settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryRecipes(queryString, results);
                }
            }

            // Add to search history
            if (settings.TrackHistory)
            {
                this.InsertSearchHistory(accountId, "Moogle Shop", "Recipes",
                    queryString ?? string.Empty, recipes ?? string.Empty,
                    games ?? string.Empty, categories ?? string.Empty, synthMaterials ?? string.Empty);
            }

            // Check for Favourites and Projects
            results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }

        private IQueryable<Recipe> MainSearchQueryRecipes(string queryString, IQueryable<Recipe> recipes)
        {
            // Combine our incoming scenes with what we find with the straight query search
            var results = _context.Recipes.AsNoTrackingWithIdentityResolution();

            var resultIds = results
                .Where(x =>
                    x.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Game.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.RecipeMaterials.Any(y => y.Inventory.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Category.ToLower().Contains(queryString.ToLower())
                ).Select(x => x.Id).ToList();

            var allIds = recipes.Select(x => x.Id).ToList().Union(resultIds);

            return results.Where(x => allIds.Contains(x.Id));
        }

        private IQueryable<Recipe> GetFavouritesProjectsSearchResults(string accountId, SearchSettings settings, IQueryable<Recipe> results)
        {
            if (settings.FavouriteSearch && settings.ProjectSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Moogle Shop" && x.Category == "Recipes").Select(x => x.SpecificRecordId);
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Moogle Shop" && x.Category == "Recipes").Select(x => x.SpecificRecordId);

                var combinedIds = favouriteIds.Union(projectRecordIds);

                results = results.Where(x => combinedIds.Contains(x.Id));
            }
            else if (settings.FavouriteSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Moogle Shop" && x.Category == "Recipes").Select(x => x.SpecificRecordId);

                results = results.Where(x => favouriteIds.Contains(x.Id));
            }
            else if (settings.ProjectSearch)
            {
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Moogle Shop" && x.Category == "Recipes").Select(x => x.SpecificRecordId);

                results = results.Where(x => projectRecordIds.Contains(x.Id));
            }

            return results;
        }

        [HttpGet("GetInventory")]
        public async Task<List<InventoryDto>> GetInventory([FromQuery] string accountId, [FromQuery] string? games = null, [FromQuery] string? items = null, [FromQuery] string? enemies = null, [FromQuery] string? worlds = null, [FromQuery] string? areas = null, [FromQuery] string? description = null, [FromQuery] string? category = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null)
            {
                settings = new SearchSettings { AccountId = accountId };

                _context.SearchSettings.Add(settings);
                _context.SaveChanges();
            }

            IQueryable<Inventory> results;
            if (category == "Weapon")
            {
                results = _context.Inventory.AsNoTrackingWithIdentityResolution().Where(x => x.Category == "Keyblade" || x.Category == "Staff" || x.Category == "Shield");
            }
            else if (category == "Accessories & Armor")
            {
                results = _context.Inventory.AsNoTrackingWithIdentityResolution().Where(x => x.Category == "Accessory" || x.Category == "Armor");
            }
            else
            {
                results = _context.Inventory.AsNoTrackingWithIdentityResolution().Where(x => x.Category == category);
            }

            var queryString = (description == null ? description : description.Any(x => char.IsPunctuation(x)) ? description : Regex.Replace(description, @"[^\w\s]", ""));

            if (items != null)
            {
                var inventoryList = items.Split("::").Select(x => x.Trim());

                results = results.Where(x => inventoryList.Contains(x.Name));
            }

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (enemies != null)
            {
                var enemiesList = enemies.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.EnemyDrops).Where(x => x.EnemyDrops.Any(y => enemiesList.Contains(y.CharacterLocation.Character.Name))))
                {
                    if (result == null || result.EnemyDrops == null) continue;

                    if (enemiesList.All(x => result.EnemyDrops.Select(y => y.CharacterLocation.Character.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (worlds != null)
            {
                var worldsList = worlds.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.EnemyDrops).Where(x => x.EnemyDrops.Any(y => worldsList.Contains(y.CharacterLocation.World.Name))))
                {
                    if (result == null || result.EnemyDrops == null) continue;

                    if (worldsList.All(x => result.EnemyDrops.Select(y => y.CharacterLocation.World.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (areas != null)
            {
                var areasList = areas.Split("::").Select(x => x.Trim());

                var resultIds = new List<int>();
                foreach (var result in results.Include(x => x.EnemyDrops).Where(x => x.EnemyDrops.Any(y => y.CharacterLocation.Areas.Any(z => areasList.Contains(z.Name)))))
                {
                    if (result == null || result.EnemyDrops == null) continue;

                    if (areasList.All(x => result.EnemyDrops.Select(y => y.CharacterLocation).SelectMany(y => y.Areas).Select(y => y.Name).Any(y => y == x)))
                    {
                        resultIds.Add(result.Id);
                    }
                }

                results = results.Where(x => resultIds.Contains(x.Id));
            }

            if (queryString != null)
            {
                if (!queryString.Contains("\""))
                {
                    queryString = queryString.ToLower();

                    var tempResults = new List<Inventory>();
                    foreach (var result in results)
                    {
                        // Description search
                        var tempDescription = result.Description;
                        if (!queryString.Any(x => char.IsPunctuation(x)))
                            tempDescription = Regex.Replace(result.Description, @"[^\w\s]", "");

                        if (tempDescription.ToLower().Contains(queryString))
                            tempResults.Add(result);

                        // Additional Information search
                        var tempAdditionalInformation = result.AdditionalInformation;
                        if (!queryString.Any(x => char.IsPunctuation(x)))
                            tempAdditionalInformation = Regex.Replace(result.AdditionalInformation, @"[^\w\s]", "");

                        if (tempAdditionalInformation.ToLower().Contains(queryString))
                            tempResults.Add(result);
                    }

                    results = results.Where(x => tempResults.Select(y => y.Id).Contains(x.Id));
                }
                else
                {
                    queryString = queryString.Replace("\"", "");

                    results = results.Where(x => x.Description.Contains(queryString) || x.AdditionalInformation.Contains(queryString));
                }

                if (settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryInventory(queryString, results);
                }
            }

            // Add to search history
            if (settings.TrackHistory)
            {
                this.InsertSearchHistory(accountId, "Moogle Shop", category ?? "Moogle Record",
                    queryString ?? string.Empty, items ?? string.Empty,
                    games ?? string.Empty, enemies ?? string.Empty, worlds ?? string.Empty, areas ?? string.Empty);
            }

            // Check for Favourites and Projects
            results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }

        private IQueryable<Inventory> MainSearchQueryInventory(string queryString, IQueryable<Inventory> inventory)
        {
            // Combine our incoming scenes with what we find with the straight query search
            var results = _context.Inventory.AsNoTrackingWithIdentityResolution();

            var resultIds = results
                .Where(x =>
                    x.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Game.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.EnemyDrops.Any(y => y.CharacterLocation.Character.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Category.ToLower().Contains(queryString.ToLower())
                ).Select(x => x.Id).ToList();

            var allIds = inventory.Select(x => x.Id).ToList().Union(resultIds);

            return results.Where(x => allIds.Contains(x.Id));
        }

        private IQueryable<Inventory> GetFavouritesProjectsSearchResults(string accountId, SearchSettings settings, IQueryable<Inventory> results)
        {
            if (settings.FavouriteSearch && settings.ProjectSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Moogle Shop" && x.Category != "Recipes").Select(x => x.SpecificRecordId);
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Moogle Shop" && x.Category != "Recipes").Select(x => x.SpecificRecordId);

                var combinedIds = favouriteIds.Union(projectRecordIds);

                results = results.Where(x => combinedIds.Contains(x.Id));
            }
            else if (settings.FavouriteSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Moogle Shop" && x.Category != "Recipes").Select(x => x.SpecificRecordId);

                results = results.Where(x => favouriteIds.Contains(x.Id));
            }
            else if (settings.ProjectSearch)
            {
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Moogle Shop" && x.Category != "Recipes").Select(x => x.SpecificRecordId);

                results = results.Where(x => projectRecordIds.Contains(x.Id));
            }

            return results;
        }
        #endregion Moogle Shop

        #region Search History And Settings
        [HttpGet("GetSearchHistory")]
        public async Task<List<SearchHistoryDto>> GetSearchHistory(string accountId)
        {
            return await this._context.SearchHistory.Where(x => x.AccountId == accountId).ToDto().ToListAsync();
        }

        [HttpGet("GetSearchHistoryWithType")]
        public async Task<List<SearchHistoryDto>> GetSearchHistory(string accountId, string type)
        {
            return await this._context.SearchHistory.Where(x => x.AccountId == accountId && x.Type == type).ToDto().ToListAsync();
        }

        [HttpPost("InsertSearchHistory")]
        private void InsertSearchHistory(string accountId, string type, string category, string textSearch, string specificSearch,
                                         string param1 = "", string param2 = "", string param3 = "", string param4 = "", string param5 = "", string param6 = "", string param7 = "")
        {
            if (this._context.SearchHistory.Count() < 250)
            {
                Data.Models.SearchHistory historyObject = new()
                {
                    AccountId = accountId,
                    Type = type,
                    Category = category,
                    TextSearch = textSearch,
                    SpecificSearch = specificSearch,
                    Param1Search = param1,
                    Param2Search = param2,
                    Param3Search = param3,
                    Param4Search = param4,
                    Param5Search = param5,
                    Param6Search = param6,
                    Param7Search = param7,
                    CreatedDate = DateTime.Now
                };

                this._context.SearchHistory.Add(historyObject);

                this._context.SaveChanges();
            }
        }

        [HttpGet("GetSearchSettings")]
        public async Task<SearchSettingsDto> GetSearchSettings(string accountId)
        {
            var settings = await this._context.SearchSettings.FirstOrDefaultAsync(x => x.AccountId == accountId);

            if (settings == null)
            {
                return new SearchSettingsDto();
            }

            return settings.ToDto();
        }

        [HttpPost("SaveSearchSettings")]
        public async Task SaveSearchSettings(string accountId, bool autoSearch, bool autoExpandFirstResult, bool mainSearchEverything, bool trackHistory, bool favourites, bool projects, bool includeAlias)
        {
            var settings = await this._context.SearchSettings.FirstOrDefaultAsync(x => x.AccountId == accountId);

            if (settings != null)
            {
                settings.AutoSearch = autoSearch;
                settings.AutoExpandFirstResult = autoExpandFirstResult;
                settings.MainSearchEverything = mainSearchEverything;
                settings.TrackHistory = trackHistory;
                settings.FavouriteSearch = favourites;
                settings.ProjectSearch = projects;
                settings.IncludeAlias = includeAlias;
            }
            else
            {
                this._context.SearchSettings.Add(new SearchSettings
                {
                    AccountId = accountId,
                    AutoSearch = autoSearch,
                    AutoExpandFirstResult = autoExpandFirstResult,
                    MainSearchEverything = mainSearchEverything,
                    TrackHistory = trackHistory,
                    FavouriteSearch = favourites,
                    ProjectSearch = projects,
                    IncludeAlias = includeAlias
                });
            }

            this._context.SaveChanges();
        }
        #endregion Search History And Settings

        #region Favourites and Projects
        [HttpGet("GetFavourites")]
        public async Task<List<FavoriteDto>> GetFavourites(string accountId)
        {
            var favourites = this._context.Favorites.Where(x => x.AccountId == accountId);

            if (favourites.Count() == 0)
                return new List<FavoriteDto>();

            return await favourites.ToDto().ToListAsync();
        }

        [HttpGet("GetFavouritesWithType")]
        public async Task<List<FavoriteDto>> GetFavourites(string accountId, string type)
        {
            var favourites = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == type);

            if (favourites.Count() == 0)
                return new List<FavoriteDto>();

            return await favourites.ToDto().ToListAsync();
        }

        [HttpGet("GetFavouritesWithTypeAndCategory")]
        public async Task<List<FavoriteDto>> GetFavourites(string accountId, string type, string category)
        {
            var favourites = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == type && x.Category == category);

            if (favourites.Count() == 0)
                return new List<FavoriteDto>();

            return await favourites.ToDto().ToListAsync();
        }

        [HttpPost("InsertRemoveFavourite")]
        public async Task InsertRemoveFavourite(string accountId, string type, string category, int specificRecordId)
        {
            Favorite favoriteObject = new()
            {
                AccountId = accountId,
                Type = type,
                Category = category,
                SpecificRecordId = specificRecordId,
                CreatedDate = DateTime.Now
            };

            Favorite? matchFavorite = await this._context.Favorites
                .FirstOrDefaultAsync(x => x.AccountId == favoriteObject.AccountId && x.Type == favoriteObject.Type && x.Category == favoriteObject.Category && x.SpecificRecordId == favoriteObject.SpecificRecordId);

            if (matchFavorite != null)
            {
                this._context.Favorites.Remove(matchFavorite);
            }
            else
            {
                this._context.Favorites.Add(favoriteObject);
            }

            this._context.SaveChanges();
        }

        [HttpGet("GetProjects")]
        public async Task<List<ProjectDto>> GetProjects(string accountId, bool includeProjectRecords = false)
        {
            if (includeProjectRecords)
            {
                return await this._context.Projects.Include(x => x.ProjectRecords).Where(x => x.AccountId == accountId).ToDto().ToListAsync();
            }

            return await this._context.Projects.Where(x => x.AccountId == accountId).ToDto().ToListAsync();
        }

        [HttpPost("InsertProject")]
        public async Task InsertProject(string accountId, string name, string description)
        {
            var project = await this._context.Projects.FirstOrDefaultAsync(x => x.AccountId == accountId && x.Name == name);

            if (project == null)
            {
                if (name.Length > 50)
                {
                    name = name[..50];
                }

                this._context.Projects.Add(new Data.Models.Project
                {
                    AccountId = accountId,
                    Name = name,
                    Description = description,
                    CreatedDate = DateTime.Now
                });

                this._context.SaveChanges();
            }

        }

        [HttpDelete("RemoveProject")]
        public async Task RemoveProject(string accountId, string name)
        {
            var project = await this._context.Projects.FirstOrDefaultAsync(x => x.AccountId == accountId && x.Name == name);

            if (project != null)
            {
                this._context.Projects.Remove(project);

                this._context.SaveChanges();
            }
        }

        [HttpPost("RenameProject")]
        public async Task RenameProject(string accountId, string originalName, string name)
        {
            var project = await this._context.Projects.FirstOrDefaultAsync(x => x.AccountId == accountId && x.Name == name);

            if (project != null)
            {
                this._context.Projects.Remove(project);

                this._context.SaveChanges();
            }
        }

        [HttpGet("GetProjectRecords")]
        public async Task<List<ProjectRecordDto>> GetProjectRecords(string accountId, string name)
        {
            try
            {
                return await this._context.Projects.Where(x => x.AccountId == accountId && x.Name == name).ToDto().SelectMany(x => x.ProjectRecords).ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return null!;
            }
        }

        [HttpPost("InsertRemoveProjectRecord")]
        public async Task InsertRemoveProjectRecord(string accountId, string name, string type, string category, int specificRecordId)
        {
            var project = await this._context.Projects.Include(x => x.ProjectRecords).FirstOrDefaultAsync(x => x.AccountId == accountId && x.Name == name);

            if (project != null)
            {
                var projectRecord = project.ProjectRecords.FirstOrDefault(x => x.Type == type && x.Category == category && x.SpecificRecordId == specificRecordId);

                if (projectRecord != null)
                {
                    project.ProjectRecords.Remove(projectRecord);
                }
                else
                {
                    project.ProjectRecords.Add(new ProjectRecord
                    {
                        CreatedDate = DateTime.Now,
                        Type = type,
                        Category = category,
                        SpecificRecordId = specificRecordId,
                        Project = project
                    });
                }

                this._context.SaveChanges();
            }
        }

        [HttpPost("EditProjectRecordNotes")]
        public async Task EditProjectRecordNotes(string accountId, string name, string type, string category, int specificRecordId, string value)
        {
            var project = await this._context.Projects.Include(x => x.ProjectRecords).FirstOrDefaultAsync(x => x.AccountId == accountId && x.Name == name);

            if (project != null)
            {
                var projectRecord = project.ProjectRecords.FirstOrDefault(x => x.Type == type && x.Category == category && x.SpecificRecordId == specificRecordId);

                if (projectRecord != null)
                {
                    projectRecord.Notes = value;

                    this._context.SaveChanges();
                }
            }
        }
        #endregion Favorites And Projects


        #region Discord Methods
        [HttpGet("GetRandomScene")]
        public async Task<SceneDto> GetRandomScene()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            var ids = await this._context.Scenes.Select(x => x.Id).ToListAsync();

            var randomScene = random.Next(0, ids.Count);

            var id = ids[randomScene];
            return (await this._context.Scenes.Include(x => x.Game).Include(x => x.Areas).Include(x => x.Characters).Include(x => x.Music).Include(x => x.Worlds).Include(x => x.Script).Include(x => x.Script.Lines)
                .FirstAsync(x => x.Id == randomScene))
                .ToDto();
        }
        #endregion Discord Methods
    }
}