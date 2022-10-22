using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<WayfinderController> _logger;
        private readonly WayfinderContext _context;

        public WayfinderController(ILogger<WayfinderController> logger, WayfinderContext context)
        {
            _logger = logger;
            _context = context;
        }


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

                    results = results.Where(x => tempResults.Select(y =>  y.Id).Contains(x.Id));
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
    }
}