using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("GetScenes")]
        public IEnumerable<SceneDto> GetScenes()
        {
            return _context.Scenes.ToDto();
        }

        [HttpGet("GetScenes/{gameName}")]
        public IEnumerable<SceneDto> GetScenes(string gameName)
        {
            return _context.Scenes
                .Where(x => x.Game.Name == gameName)
                .ToDto();
        }

        [HttpGet("SearchForScenes/{gameName}/{characters}")]
        public IEnumerable<SceneDto> SearchForScenes(string gameName, string characters)
        {
            var charactersList = characters.Split(",").Select(x => x.Trim());

            return _context.Scenes
                .Where(x => x.Game.Name == gameName && 
                        x.Characters.Any(y => charactersList.Contains(y.Name)))
                .ToDto();
        }

        [HttpGet("SearchForScenes/{gameName}/{worlds}/{characters}")]
        public IEnumerable<SceneDto> SearchForScenes(string gameName, string worlds, string characters)
        {
            var worldsList = worlds.Split(",").Select(x => x.Trim());
            var charactersList = characters.Split(",").Select(x => x.Trim());

            return _context.Scenes
                .Where(x => x.Game.Name == gameName &&
                        x.Worlds.Any(y => worldsList.Contains(y.Name)) &&
                        x.Characters.Any(y => charactersList.Contains(y.Name)))
                .ToDto();
        }

        [HttpGet("SearchForScenes/{gameName}/{worlds}/{characters}/{line}")]
        public IEnumerable<SceneDto> SearchForScenes(string gameName, string worlds, string characters, string line)
        {
            var worldsList = worlds.Split(",").Select(x => x.Trim());
            var charactersList = characters.Split(",").Select(x => x.Trim());

            return _context.Scenes
                .Where(x => x.Game.Name == gameName &&
                        x.Worlds.Any(y => worldsList.Contains(y.Name)) &&
                        x.Characters.Any(y => charactersList.Contains(y.Name)) &&
                        x.Script.Lines.Any(y => y.Line.ToLower().Contains(line.ToLower())))
                .ToDto();
        }

        [HttpGet("SearchForScenes")]
        public async Task<IEnumerable<SceneDto>> SearchForScenes([FromQuery] string? games = null, [FromQuery] string? scenes = null, [FromQuery] string? worlds = null, [FromQuery] string? characters = null, [FromQuery] string? areas = null, [FromQuery] string? music = null, [FromQuery] string? line = null)
        {
            var results = _context.Scenes.AsNoTrackingWithIdentityResolution();

            if (games != null)
            {
                var gamesList = games.Split(",").Select(x => x.Trim());

                results = results.Where(x => gamesList.Any(y => y == x.Game.Name));
            }

            if (scenes != null)
            {
                var scenesList = scenes.Split(",").Select(x => x.Trim());

                results = results.Where(x => scenesList.Any(y => y == x.Name));
            }
            
            if (worlds != null)
            {
                var worldsList = worlds.Split(",").Select(x => x.Trim());

                results = results.Where(x => x.Worlds.Any(y => worldsList.Any(z => z == y.Name)));
            }

            if (characters != null)
            {
                var charactersList = characters.Split(",").Select(x => x.Trim());

                results = results.Where(x => x.Characters.Any(y => charactersList.Any(z => z == y.Name)));
            }

            if (areas != null)
            {
                var areasList = areas.Split(",").Select(x => x.Trim());

                results = results.Where(x => x.Areas.Any(y => areasList.Any(z => z == y.Name)));
            }

            if (music != null)
            {
                var musicList = music.Split(",").Select(x => x.Trim());

                results = results.Where(x => x.Music.Any(y => musicList.Any(z => z == y.Name)));
            }

            if (line != null)
            {
                results = results.Where(x => x.Script.Lines.ToList().Any(y => y.Line.ToLower().Contains(line.ToLower())));
            }

            return await results.OrderBy(x => x.Id).ToDto().ToListAsync();
        }
    }
}