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
        [HttpGet("SearchForScenes")]
        public async Task<List<SceneDto>> SearchForScenes([FromQuery] string? games = null, [FromQuery] string? scenes = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? areas = null, [FromQuery] string? music = null, [FromQuery] string? line = null)
        {
            var results = _context.Scenes.AsNoTrackingWithIdentityResolution();
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
            }

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
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
        public async Task<List<InterviewDto>> GetInterviews([FromQuery] string? interviews = null, [FromQuery] string? games = null, [FromQuery] string? participants = null, [FromQuery] string? providers = null, [FromQuery] string? translators = null, [FromQuery] string? line = null)
        {
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
            }

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
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
        public async Task<List<JournalEntryDto>> SearchForJournalEntries([FromQuery] string? games = null, [FromQuery] string? entries = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? information = null, [FromQuery] string? category = null)
        {
            var results = _context.JournalEntries.AsNoTrackingWithIdentityResolution().Where(x => x.Category == category);
            var queryString = (information == null ? information : information.Any(x => char.IsPunctuation(x)) ? information : Regex.Replace(information, @"[^\w\s]", ""));

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (entries != null)
            {
                var titlesList = entries.Split("::").Select(x => x.Trim());

                results = results.Where(x => titlesList.Contains(x.Title));
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
            }

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
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
        public async Task<List<RecipeDto>> GetRecipes([FromQuery] string? games = null, [FromQuery] string? recipes = null, [FromQuery] string? synthMaterials = null, [FromQuery] string? description = null, [FromQuery] string? categories = null)
        {
            var results = _context.Recipes.AsNoTrackingWithIdentityResolution();
            var queryString = (description == null ? description : description.Any(x => char.IsPunctuation(x)) ? description : Regex.Replace(description, @"[^\w\s]", ""));

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (recipes != null)
            {
                var recipeNamesList = recipes.Split("::").Select(x => x.Trim());

                results = results.Where(x => recipeNamesList.Contains(x.Name));
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
            }

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }

        [HttpGet("GetInventory")]
        public async Task<List<InventoryDto>> GetInventory([FromQuery] string? games = null, [FromQuery] string? items = null, [FromQuery] string? enemies = null, [FromQuery] string? worlds = null, [FromQuery] string? areas = null, [FromQuery] string? description = null, [FromQuery] string? category = null)
        {
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

            if (games != null)
            {
                var gamesList = games.Split("::").Select(x => x.Trim());

                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (items != null)
            {
                var inventoryList = items.Split("::").Select(x => x.Trim());

                results = results.Where(x => inventoryList.Contains(x.Name));
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
            }

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }
        #endregion Moogle Shop
    }
}