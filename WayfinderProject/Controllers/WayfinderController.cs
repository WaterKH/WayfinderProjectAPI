using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WayfinderProjectAPI.Data;
using WayfinderProjectAPI.Data.DTOs;
using WayfinderProjectAPI.Data.Models;
using static WayfinderProjectAPI.Data.DatabaseInitializer;

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
        public async Task<List<SceneDto>> GetScenes([FromQuery] string? accountId, [FromQuery] string? games = null, [FromQuery] string? scenes = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? areas = null, [FromQuery] string? music = null, [FromQuery] string? line = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null && accountId != null)
            {
                settings = new SearchSettings { AccountId = accountId };

                _context.SearchSettings.Add(settings);
                _context.SaveChanges();
            }

            var results = _context.Scenes.Include(x => x.Worlds).Include(x => x.Areas).Include(x => x.Characters).Include(x => x.Music).AsNoTrackingWithIdentityResolution();

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

                if (settings != null && settings.IncludeAlias)
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
                    if (settings != null && settings.IncludeAlias)
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

                if (settings != null && settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryScenes(queryString, results);
                }
            }

            if (accountId != null && settings != null)
            {
                // Add to search history
                if (settings.TrackHistory)
                {
                    this.InsertSearchHistory(accountId, "Memory Archive", "Scenes",
                        queryString ?? string.Empty, scenes ?? string.Empty,
                        games ?? string.Empty, worlds ?? string.Empty, areas ?? string.Empty, characters ?? string.Empty, music ?? string.Empty);
                }

                // Check for Favourites and Projects
                results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);
            }

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

        [HttpPost("AddScene")]
        public void AddScene([FromQuery] string? accountId, [FromQuery] string? sceneName = null, [FromQuery] string? sceneLink = null, [FromQuery] string? gameName = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? areas = null, [FromQuery] string? music = null, [FromQuery] string? script = null)
        {
            var user = _context.Users.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.Id == accountId);
            if (user == null || !Extensions.IsAdmin(user))
            {
                return;
            }

            var scene = _context.Scenes.Include(x => x.Worlds).Include(x => x.Characters).Include(x => x.Areas).Include(x => x.Music).FirstOrDefault(x => x.Name == sceneName && x.Game.Name == gameName);

            if (scene == null)
            {
                scene = new Scene();
            }

            scene.Name = sceneName ?? "ERRORED SCENE NAME";
            scene.Game = _context.Games.FirstOrDefault(x => x.Name == gameName);
            scene.Link = sceneLink;

            if (worlds != null)
            {
                scene.Worlds?.Clear();
                _context.SaveChanges();

                var splitWorlds = worlds.Split("::");

                scene.Worlds = _context.Worlds.Where(x => splitWorlds.Contains(x.Name)).ToList();
            }

            if (characters != null)
            {
                scene.Characters?.Clear();
                _context.SaveChanges();

                var splitCharacters = characters.Split("::");

                scene.Characters = _context.Characters.Where(x => splitCharacters.Contains(x.Name)).ToList();
            }

            if (areas != null)
            {
                scene.Areas?.Clear();
                _context.SaveChanges();

                var splitAreas = areas.Split("::");

                scene.Areas = _context.Areas.Where(x => splitAreas.Contains(x.Name)).ToList();
            }

            if (music != null)
            {
                scene.Music?.Clear();
                _context.SaveChanges();

                var splitMusic = music.Split("::");

                scene.Music = _context.Music.Where(x => splitMusic.Contains(x.Name)).ToList();
            }

            _context.SaveChanges();

            // Update Script
            if (script != null)
            {
                var splitScript = script.Split("::");
                var sceneScript = _context.Script.FirstOrDefault(x => x.SceneName == scene.Name && x.GameName == scene.Game.Name);

                if (sceneScript == null)
                {
                    if (splitScript.Length == 1 && splitScript[0] == "None: None")
                    {
                        scene.Script = _context.Script.FirstOrDefault(x => x.Id == 1); // None

                        _context.SaveChanges();

                        return;
                    }

                    sceneScript = new Script
                    {
                        SceneName = scene.Name,
                        GameName = scene.Game.Name,
                        Lines = new List<ScriptLine>()
                    };

                    _context.Add(sceneScript);
                    _context.SaveChanges();

                    sceneScript = _context.Script.First(x => x.SceneName == scene.Name && x.GameName == scene.Game.Name);
                }
                else
                {
                    sceneScript.Lines = new List<ScriptLine>();
                }

                // Remove previous line in this position
                var tempScriptLines = _context.ScriptLine.Where(x => x.Script.Id == sceneScript.Id);

                if (tempScriptLines != null)
                {
                    _context.RemoveRange(tempScriptLines);
                    _context.SaveChanges();
                }

                for (int i = 0; i < splitScript.Length; ++i)
                {
                    var subSplitScript = splitScript[i].Split(": ");

                    if (subSplitScript.Length < 2) continue;

                    //var scriptLine = _context.ScriptLine.FirstOrDefault(x => x.Script.Id == sceneScript.Id && x.Character == subSplitScript[0] && x.Line == subSplitScript[1]);

                    //if (scriptLine == null)
                    //{
                    var scriptLine = new ScriptLine
                    {
                        Order = i,
                        Character = subSplitScript[0],
                        Line = subSplitScript[1]
                    };

                    sceneScript.Lines.Add(scriptLine);
                    //}
                    //else
                    //{
                    //scriptLine.Order = i;
                    //scriptLine.Character = subSplitScript[0];
                    //scriptLine.Line = subSplitScript[1];

                    //sceneScript.Lines.Add(scriptLine);
                    //}
                }

                scene.Script = sceneScript;

                _context.SaveChanges();
            }
        }

        [HttpGet("GetInteractions")]
        public async Task<List<InteractionDto>> GetInteractions([FromQuery] string? accountId, [FromQuery] string? games = null, [FromQuery] string? interactions = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? areas = null, [FromQuery] string? music = null, [FromQuery] string? line = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null && accountId != null)
            {
                settings = new SearchSettings { AccountId = accountId };

                _context.SearchSettings.Add(settings);
                _context.SaveChanges();
            }

            var results = _context.Interactions.AsNoTrackingWithIdentityResolution();

            // See if we search for favourites

            var queryString = (line == null ? line : line.Any(x => char.IsPunctuation(x)) ? line : Regex.Replace(line, @"[^\w\s]", ""));

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (interactions != null)
            {
                var interactionsList = interactions.Split("::").Select(x => x.Trim());

                results = results.Where(x => interactionsList.Contains(x.Name));
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

                if (settings != null && settings.IncludeAlias)
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
                    if (settings != null && settings.IncludeAlias)
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

                    var tempResults = new List<Data.Models.Interaction>();
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

                if (settings != null && settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryInteractions(queryString, results);
                }
            }

            if (accountId != null && settings != null)
            {
                // Add to search history
                if (settings.TrackHistory)
                {
                    this.InsertSearchHistory(accountId, "Memory Archive", "Interactions",
                        queryString ?? string.Empty, interactions ?? string.Empty,
                        games ?? string.Empty, worlds ?? string.Empty, areas ?? string.Empty, characters ?? string.Empty, music ?? string.Empty);
                }

                // Check for Favourites and Projects
                results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);
            }

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }

        private IQueryable<Interaction> MainSearchQueryInteractions(string queryString, IQueryable<Interaction> interactions)
        {
            // Combine our incoming interactions with what we find with the straight query search
            var results = _context.Interactions.AsNoTrackingWithIdentityResolution();

            var resultIds = results
                .Where(x =>
                    x.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Game.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Areas.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Characters.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Music.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Worlds.Any(y => y.Name.ToLower().Contains(queryString.ToLower()))
                ).Select(x => x.Id).ToList();

            var allIds = interactions.Select(x => x.Id).ToList().Union(resultIds);

            return results.Where(x => allIds.Contains(x.Id));
        }

        private IQueryable<Interaction> GetFavouritesProjectsSearchResults(string accountId, SearchSettings settings, IQueryable<Interaction> results)
        {
            if (settings.FavouriteSearch && settings.ProjectSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Memory Archive" && x.Category == "Interactions").Select(x => x.SpecificRecordId);
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Memory Archive" && x.Category == "Interactions").Select(x => x.SpecificRecordId);

                var combinedIds = favouriteIds.Union(projectRecordIds);

                results = results.Where(x => combinedIds.Contains(x.Id));
            }
            else if (settings.FavouriteSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Memory Archive" && x.Category == "Interactions").Select(x => x.SpecificRecordId);

                results = results.Where(x => favouriteIds.Contains(x.Id));
            }
            else if (settings.ProjectSearch)
            {
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Memory Archive" && x.Category == "Interactions").Select(x => x.SpecificRecordId);

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
                var scene = await _context.Scenes.AsNoTrackingWithIdentityResolution().Include(x => x.Script).FirstOrDefaultAsync(x => x.Game.Name == gameName && x.Name == sceneName);

                if (scene != null)
                {
                    result = await _context.Script.AsNoTrackingWithIdentityResolution().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == scene.Script.Id);
                }

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
            }

            return result.ToDto();
        }


        [HttpGet("GetTrailers")]
        public async Task<List<TrailerDto>> GetTrailers([FromQuery] string? accountId, [FromQuery] string? games = null, [FromQuery] string? trailers = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? areas = null, [FromQuery] string? music = null, [FromQuery] string? line = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null && accountId != null)
            {
                settings = new SearchSettings { AccountId = accountId };

                _context.SearchSettings.Add(settings);
                _context.SaveChanges();
            }

            var results = _context.Trailers.AsNoTrackingWithIdentityResolution();

            // See if we search for favourites

            var queryString = (line == null ? line : line.Any(x => char.IsPunctuation(x)) ? line : Regex.Replace(line, @"[^\w\s]", ""));

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (trailers != null)
            {
                var trailersList = trailers.Split("::").Select(x => x.Trim());

                results = results.Where(x => trailersList.Contains(x.Name));
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

                if (settings != null && settings.IncludeAlias)
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
                    if (settings != null && settings.IncludeAlias)
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

                    var tempResults = new List<Data.Models.Trailer>();
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

                if (settings != null && settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryTrailers(queryString, results);
                }
            }

            if (accountId != null && settings != null)
            {
                // Add to search history
                if (settings.TrackHistory)
                {
                    this.InsertSearchHistory(accountId, "Memory Archive", "Trailers",
                        queryString ?? string.Empty, trailers ?? string.Empty,
                        games ?? string.Empty, worlds ?? string.Empty, areas ?? string.Empty, characters ?? string.Empty, music ?? string.Empty);
                }

                // Check for Favourites and Projects
                results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);
            }

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }

        private IQueryable<Trailer> MainSearchQueryTrailers(string queryString, IQueryable<Trailer> trailers)
        {
            // Combine our incoming Trailers with what we find with the straight query search
            var results = _context.Trailers.AsNoTrackingWithIdentityResolution();

            var resultIds = results
                .Where(x =>
                    x.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Game.Name.ToLower().Contains(queryString.ToLower()) ||
                    x.Areas.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Characters.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Music.Any(y => y.Name.ToLower().Contains(queryString.ToLower())) ||
                    x.Worlds.Any(y => y.Name.ToLower().Contains(queryString.ToLower()))
                ).Select(x => x.Id).ToList();

            var allIds = trailers.Select(x => x.Id).ToList().Union(resultIds);

            return results.Where(x => allIds.Contains(x.Id));
        }

        private IQueryable<Trailer> GetFavouritesProjectsSearchResults(string accountId, SearchSettings settings, IQueryable<Trailer> results)
        {
            if (settings.FavouriteSearch && settings.ProjectSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Memory Archive" && x.Category == "Trailers").Select(x => x.SpecificRecordId);
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Memory Archive" && x.Category == "Trailers").Select(x => x.SpecificRecordId);

                var combinedIds = favouriteIds.Union(projectRecordIds);

                results = results.Where(x => combinedIds.Contains(x.Id));
            }
            else if (settings.FavouriteSearch)
            {
                var favouriteIds = this._context.Favorites.Where(x => x.AccountId == accountId && x.Type == "Memory Archive" && x.Category == "Trailers").Select(x => x.SpecificRecordId);

                results = results.Where(x => favouriteIds.Contains(x.Id));
            }
            else if (settings.ProjectSearch)
            {
                var projectRecordIds = this._context.Projects.Where(x => x.AccountId == accountId).SelectMany(x => x.ProjectRecords).Where(x => x.Type == "Memory Archive" && x.Category == "Trailers").Select(x => x.SpecificRecordId);

                results = results.Where(x => projectRecordIds.Contains(x.Id));
            }

            return results;
        }

        [HttpGet("GetInterviews")]
        public async Task<List<InterviewDto>> GetInterviews([FromQuery] string? accountId, [FromQuery] string? interviews = null, [FromQuery] string? games = null, [FromQuery] string? participants = null, [FromQuery] string? providers = null, [FromQuery] string? translators = null, [FromQuery] string? line = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null && accountId != null)
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

                if (settings != null && settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryInterviews(queryString, results);
                }
            }

            if (accountId != null && settings != null)
            {
                // Add to search history
                if (settings.TrackHistory)
                {
                    this.InsertSearchHistory(accountId, "Memory Archive", "Interviews",
                        queryString ?? string.Empty, interviews ?? string.Empty,
                        games ?? string.Empty, participants ?? string.Empty, providers ?? string.Empty, translators ?? string.Empty);
                }

                // Check for Favourites and Projects
                results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);
            }

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
        [HttpGet("GetJournalEntries")]
        public async Task<List<JournalEntryDto>> GetJournalEntries([FromQuery] string? accountId, [FromQuery] string? games = null, [FromQuery] string? entries = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? information = null, [FromQuery] string? category = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null && accountId != null)
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

                if (settings != null && settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryJournalEntries(queryString, results);
                }
            }

            if (accountId != null && settings != null)
            {
                // Add to search history
                if (settings.TrackHistory)
                {
                    this.InsertSearchHistory(accountId, "Jiminy's Journal", category ?? "Journal Entry",
                        queryString ?? string.Empty, entries ?? string.Empty,
                        games ?? string.Empty, characters ?? string.Empty, worlds ?? string.Empty);
                }

                // Check for Favourites and Projects
                results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);
            }

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
        public async Task<List<RecipeDto>> GetRecipes([FromQuery] string? accountId, [FromQuery] string? games = null, [FromQuery] string? recipes = null, [FromQuery] string? synthMaterials = null, [FromQuery] string? description = null, [FromQuery] string? categories = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null && accountId != null)
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

                if (settings != null && settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryRecipes(queryString, results);
                }
            }

            if (accountId != null && settings != null)
            {
                // Add to search history
                if (settings.TrackHistory)
                {
                    this.InsertSearchHistory(accountId, "Moogle Shop", "Recipes",
                        queryString ?? string.Empty, recipes ?? string.Empty,
                        games ?? string.Empty, categories ?? string.Empty, synthMaterials ?? string.Empty);
                }

                // Check for Favourites and Projects
                results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);
            }

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
        public async Task<List<InventoryDto>> GetInventory([FromQuery] string? accountId, [FromQuery] string? games = null, [FromQuery] string? items = null, [FromQuery] string? enemies = null, [FromQuery] string? worlds = null, [FromQuery] string? areas = null, [FromQuery] string? description = null, [FromQuery] string? category = null)
        {
            var settings = _context.SearchSettings.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.AccountId == accountId);
            if (settings == null && accountId != null)
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

                if (settings != null && settings.MainSearchEverything)
                {
                    results = this.MainSearchQueryInventory(queryString, results);
                }
            }

            if (accountId != null && settings != null)
            {
                // Add to search history
                if (settings.TrackHistory)
                {
                    this.InsertSearchHistory(accountId, "Moogle Shop", category ?? "Moogle Record",
                        queryString ?? string.Empty, items ?? string.Empty,
                        games ?? string.Empty, enemies ?? string.Empty, worlds ?? string.Empty, areas ?? string.Empty);
                }

                // Check for Favourites and Projects
                results = this.GetFavouritesProjectsSearchResults(accountId, settings, results);
            }

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
        public async Task SaveSearchSettings(string accountId, bool autoSearch, bool autoExpandFirstResult, bool mainSearchEverything, bool trackHistory, bool favourites, bool projects, bool includeAlias, string resultsDisplay)
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
                settings.ResultsDisplay = resultsDisplay;
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
                    IncludeAlias = includeAlias,
                    ResultsDisplay = resultsDisplay
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
        [HttpGet("GetDailyScene")]
        public async Task<SceneDto> GetDailyScene()
        {
            var dailyScene = await this._context.DailyCutscenes.OrderByDescending(x => x.Id).FirstAsync();

            return (await this._context.Scenes.Include(x => x.Game).Include(x => x.Areas).Include(x => x.Characters).Include(x => x.Music).Include(x => x.Worlds).Include(x => x.Script).Include(x => x.Script.Lines)
                .FirstAsync(x => x.Id == dailyScene.SceneId))
                .ToDto();
        }

        [HttpGet("GetRandomScene")]
        public async Task<SceneDto> GetRandomScene()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            var ids = await this._context.Scenes.Select(x => x.Id).ToListAsync();

            var randomScene = random.Next(0, ids.Count);

            var id = ids[randomScene];
            return (await this._context.Scenes.Include(x => x.Game).Include(x => x.Areas).Include(x => x.Characters).Include(x => x.Music).Include(x => x.Worlds).Include(x => x.Script).Include(x => x.Script.Lines)
                .FirstAsync(x => x.Id == id))
                .ToDto();
        }

        [HttpGet("GetScene")]
        public async Task<SceneDto> GetScene(int sceneId)
        {
            var scene = await this._context.Scenes.AsNoTrackingWithIdentityResolution()
                .Include(x => x.Game).Include(x => x.Areas).Include(x => x.Characters).Include(x => x.Music).Include(x => x.Worlds).Include(x => x.Script).Include(x => x.Script.Lines)
                .FirstOrDefaultAsync(x => x.Id == sceneId);

            if (scene == null)
            {
                return new SceneDto();
            }

            return scene.ToDto();
        }

        [HttpGet("GetRandomInterview")]
        public async Task<InterviewDto> GetRandomInterview()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            var ids = await this._context.Interviews.Select(x => x.Id).ToListAsync();

            var randomInterview = random.Next(0, ids.Count);

            var id = ids[randomInterview];
            return (await this._context.Interviews.Include(x => x.Games).Include(x => x.Translator).Include(x => x.Participants).Include(x => x.Conversation).Include(x => x.Provider)
                .FirstAsync(x => x.Id == id))
                .ToDto();
        }

        [HttpGet("GetInterview")]
        public async Task<InterviewDto> GetInterview(int interviewId)
        {
            var interview = await this._context.Interviews.AsNoTrackingWithIdentityResolution()
                .Include(x => x.Games).Include(x => x.Translator).Include(x => x.Participants).Include(x => x.Conversation).Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == interviewId);

            if (interview == null)
            {
                return new InterviewDto();
            }

            return interview.ToDto();
        }

        [HttpGet("GetDailyEntry")]
        public async Task<JournalEntryDto> GetDailyEntry()
        {
            var dailyEntry = await this._context.DailyJournalEntries.OrderByDescending(x => x.Id).FirstAsync();

            return (await this._context.JournalEntries.Include(x => x.Game).Include(x => x.Characters).Include(x => x.Worlds)
                .FirstAsync(x => x.Id == dailyEntry.Id))
                .ToDto();
        }

        [HttpGet("GetRandomEntry")]
        public async Task<JournalEntryDto> GetRandomEntry()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            var ids = await this._context.JournalEntries.Select(x => x.Id).ToListAsync();

            var randomEntry = random.Next(0, ids.Count);

            var id = ids[randomEntry];
            return (await this._context.JournalEntries.Include(x => x.Game).Include(x => x.Characters).Include(x => x.Worlds)
                .FirstAsync(x => x.Id == id))
                .ToDto();
        }

        [HttpGet("GetEntry")]
        public async Task<JournalEntryDto> GetEntry(int entryId)
        {
            var entry = await this._context.JournalEntries.AsNoTrackingWithIdentityResolution()
                .Include(x => x.Game).Include(x => x.Characters).Include(x => x.Worlds)
                .FirstOrDefaultAsync(x => x.Id == entryId);

            if (entry == null)
            {
                return new JournalEntryDto();
            }

            return entry.ToDto();
        }

        [HttpGet("GetRandomRecipe")]
        public async Task<RecipeDto> GetRandomRecipe()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            var ids = await this._context.Recipes.Select(x => x.Id).ToListAsync();

            var randomRecipe = random.Next(0, ids.Count);

            var id = ids[randomRecipe];
            var recipe = this._context.Recipes.Include(x => x.Game).Include(x => x.RecipeMaterials)
                .Where(x => x.Id == id);

            return await recipe.ToDto().FirstAsync();
        }

        [HttpGet("GetRecipe")]
        public async Task<RecipeDto> GetRecipe(int recipeId)
        {
            var recipe = this._context.Recipes.AsNoTrackingWithIdentityResolution()
                .Include(x => x.Game).Include(x => x.RecipeMaterials)
                .Where(x => x.Id == recipeId);

            if (recipe == null)
            {
                return new RecipeDto();
            }

            return await recipe.Take(1).ToDto().FirstAsync();
        }

        [HttpGet("GetDailyRecord")]
        public async Task<InventoryDto> GetDailyRecord()
        {
            var dailyRecord = await this._context.DailyMoogleRecords.OrderByDescending(x => x.Id).FirstAsync();

            var inventory = this._context.Inventory.Include(x => x.Game).Include(x => x.EnemyDrops)
                .Where(x => x.Id == dailyRecord.Id);

            return await inventory.Take(1).ToDto().FirstAsync();
        }

        [HttpGet("GetRandomRecord")]
        public async Task<InventoryDto> GetRandomRecord()
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            var ids = await this._context.Inventory.Select(x => x.Id).ToListAsync();

            var randomInventory = random.Next(0, ids.Count);

            var id = ids[randomInventory];
            var inventory = this._context.Inventory.Include(x => x.Game).Include(x => x.EnemyDrops)
                .Where(x => x.Id == id);

            return await inventory.Take(1).ToDto().FirstAsync();
        }

        [HttpGet("GetRecord")]
        public async Task<InventoryDto> GetRecord(int inventoryId)
        {
            var inventory = this._context.Inventory.AsNoTrackingWithIdentityResolution()
                .Include(x => x.Game).Include(x => x.EnemyDrops)
                .Where(x => x.Id == inventoryId);

            if (inventory == null)
            {
                return new InventoryDto();
            }

            return await inventory.Take(1).ToDto().FirstAsync();
        }
        #endregion Discord Methods

        #region Creation method
        [HttpPost("PatPoint")]
        public Task<bool> PatPoint([FromBody] string data)
        {
            try
            {
                var splitData = data.Split("\r\n");
                var documentType = splitData[0].Split(": ");

                switch (documentType[1])
                {
                    case "Scene":
                        var sceneGameName = splitData[1].Split(": ")[1];
                        DatabaseInitializer.CreateGame(_context, sceneGameName);

                        var sceneData = splitData.Skip(2).ToArray();
                        List<SceneObject> sceneObjects = Utilities.ProcessScenes(sceneData);

                        foreach (var sceneObject in sceneObjects)
                        {
                            sceneObject.Areas.ForEach(x => DatabaseInitializer.CreateArea(_context, x));
                            sceneObject.Characters.ForEach(x => DatabaseInitializer.CreateCharacter(_context, x));
                            sceneObject.Music.ForEach(x => DatabaseInitializer.CreateMusic(_context, new MusicObject { Name = x }));
                            sceneObject.Worlds.ForEach(x => DatabaseInitializer.CreateWorld(_context, x));

                            DatabaseInitializer.CreateScenes(_context, sceneGameName, sceneObject);
                        }
                        break;
                    case "Script":
                        var scriptGameName = splitData[1].Split(": ")[1];
                        var scriptData = splitData.Skip(2).ToArray();
                        Dictionary<string, List<LineScriptObject>> lineScriptObjects = Utilities.ProcessScript(scriptData);

                        foreach (var lineScriptObject in lineScriptObjects)
                        {
                            DatabaseInitializer.CreateScript(_context, scriptGameName, lineScriptObject.Key, lineScriptObject.Value);
                        }
                        break;
                    case "Interview":
                        var interviewData = splitData.Skip(1).ToArray();
                        InterviewObject interviewObject = Utilities.ProcessInterview(interviewData);

                        DatabaseInitializer.CreateInterviews(_context, interviewObject);
                        break;
                    case "Interaction":
                        var interactionGameName = splitData[1].Split(": ")[1];
                        DatabaseInitializer.CreateGame(_context, interactionGameName);

                        var interactionData = splitData.Skip(2).ToArray();
                        List<InteractionObject> interactionObjects = Utilities.ProcessInteractions(interactionData);

                        foreach (var interactionObject in interactionObjects)
                        {
                            interactionObject.Areas.ForEach(x => DatabaseInitializer.CreateArea(_context, x));
                            interactionObject.Characters.ForEach(x => DatabaseInitializer.CreateCharacter(_context, x));
                            interactionObject.Music.ForEach(x => DatabaseInitializer.CreateMusic(_context, new MusicObject { Name = x }));
                            interactionObject.Worlds.ForEach(x => DatabaseInitializer.CreateWorld(_context, x));

                            DatabaseInitializer.CreateInteractions(_context, interactionGameName, interactionObject);
                        }
                        break;
                    case "Trailer":
                        var trailerGameName = splitData[1].Split(": ")[1];
                        DatabaseInitializer.CreateGame(_context, trailerGameName);

                        var trailerData = splitData.Skip(2).ToArray();
                        List<TrailerObject> trailerObjects = Utilities.ProcessTrailers(trailerData);

                        foreach (var trailerObject in trailerObjects)
                        {
                            trailerObject.Areas.ForEach(x => DatabaseInitializer.CreateArea(_context, x));
                            trailerObject.Characters.ForEach(x => DatabaseInitializer.CreateCharacter(_context, x));
                            trailerObject.Music.ForEach(x => DatabaseInitializer.CreateMusic(_context, new MusicObject { Name = x }));
                            trailerObject.Worlds.ForEach(x => DatabaseInitializer.CreateWorld(_context, x));

                            DatabaseInitializer.CreateTrailers(_context, trailerGameName, trailerObject);
                        }
                        break;

                    case "Journal":
                        var journalGameName = splitData[1].Split(": ")[1];
                        var journalCategory = splitData[1].Split(": ")[2];
                        DatabaseInitializer.CreateGame(_context, journalGameName);

                        var journalData = splitData.Skip(3).ToArray();
                        List<JJCharacterObject> journalObjects = new List<JJCharacterObject>();

                        if (journalCategory == "Character")
                            journalObjects = Utilities.ProcessCharacter(journalData);
                        else if (journalCategory == "Story")
                            journalObjects = Utilities.ProcessStory(journalData);
                        else if (journalCategory == "Report")
                            journalObjects = Utilities.ProcessReports(journalData);
                        else if (journalCategory == "Enemy")
                            journalObjects = Utilities.ProcessEnemy(journalData);

                        foreach (var journalObject in journalObjects)
                        {
                            if (journalCategory != "Character")
                                journalObject.Characters.ForEach(x => DatabaseInitializer.CreateCharacter(_context, x));

                            journalObject.Worlds.ForEach(x => DatabaseInitializer.CreateWorld(_context, x));

                            DatabaseInitializer.CreateEntries(_context, journalGameName, journalObject, journalCategory);
                        }
                        break;

                    case "Item":
                        var itemGameName = splitData[1].Split(": ")[1];
                        DatabaseInitializer.CreateGame(_context, itemGameName);

                        var itemData = splitData.Skip(2).ToArray();
                        List<MSInventoryObject> itemObjects = Utilities.ProcessInventory(itemData);

                        foreach (var itemObject in itemObjects)
                        {
                            DatabaseInitializer.CreateInventory(_context, itemGameName, itemObject);
                        }
                        break;
                    case "Recipe":
                        var recipeGameName = splitData[1].Split(": ")[1];
                        DatabaseInitializer.CreateGame(_context, recipeGameName);

                        var recipeData = splitData.Skip(2).ToArray();
                        List<MSRecipeObject> recipeObjects = Utilities.ProcessRecipe(recipeData);

                        foreach (var recipeObject in recipeObjects)
                        {
                            DatabaseInitializer.CreateRecipes(_context, recipeGameName, recipeObject);
                        }
                        break;
                    case "Location":
                        var locationGameName = splitData[1].Split(": ")[1];
                        DatabaseInitializer.CreateGame(_context, locationGameName);

                        var locationData = splitData.Skip(2).ToArray();
                        List<MSMiscEnemyObject> locationObjects = Utilities.ProcessEnemyLocation(locationData);

                        foreach (var locationObject in locationObjects)
                        {
                            DatabaseInitializer.CreateCharacter(_context, locationObject.CharacterName);
                            foreach (var wa in locationObject.WorldWithAreas)
                            {
                                DatabaseInitializer.CreateWorld(_context, wa.Key);
                                wa.Value.ForEach(x => DatabaseInitializer.CreateArea(_context, x));
                            }

                            DatabaseInitializer.CreateEnemyLocations(_context, locationGameName, locationObject);
                        }
                        break;

                    default:
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
        #endregion
    }
}