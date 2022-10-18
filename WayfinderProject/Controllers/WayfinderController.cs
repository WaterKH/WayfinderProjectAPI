using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;
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
                var gamesList = games.Split(",").Select(x => x.Trim());
                
                results = results.Where(x => gamesList.Contains(x.Game.Name));
            }

            if (scenes != null)
            {
                var scenesList = scenes.Split(",").Select(x => x.Trim());

                results = results.Where(x => scenesList.Contains(x.Name));
            }
            
            if (worlds != null)
            {
                var worldsList = worlds.Split(",").Select(x => x.Trim());
                var contextWorlds = _context.Worlds.AsNoTrackingWithIdentityResolution().Where(x => worldsList.Contains(x.Name));

                results = results.Where(x => !contextWorlds.Except(x.Worlds).Any());
            }

            if (characters != null)
            {
                var charactersList = characters.Split(",").Select(x => x.Trim());
                var contextCharacters = _context.Characters.AsNoTrackingWithIdentityResolution().Where(x => charactersList.Contains(x.Name));

                results = results.Where(x => !contextCharacters.Except(x.Characters).Any());
            }

            if (areas != null)
            {
                var areasList = areas.Split(",").Select(x => x.Trim());
                var contextAreas = _context.Areas.AsNoTrackingWithIdentityResolution().Where(x => areasList.Contains(x.Name));

                results = results.Where(x => !contextAreas.Except(x.Areas).Any());
            }

            if (music != null)
            {
                var musicList = music.Split(",").Select(x => x.Trim());
                var contextMusic = _context.Music.AsNoTrackingWithIdentityResolution().Where(x => musicList.Contains(x.Name));

                results = results.Where(x => !contextMusic.Except(x.Music).Any());
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
    }
}