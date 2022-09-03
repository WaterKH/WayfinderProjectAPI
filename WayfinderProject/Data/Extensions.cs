using WayfinderProjectAPI.Data.DTOs;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Data
{
    public static class Extensions
    {
        public static IQueryable<SceneDto> ToDto(this IQueryable<Scene> scenes)
        {
            return scenes
                .Select(x => new SceneDto
                {
                    Id = x.Id,
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name },
                    Name = x.Name,
                    Link = x.Link,
                    Worlds = x.Worlds.Select(y => new WorldDto { Id = y.Id, Name = y.Name }).ToList(),
                    Characters = x.Characters.Select(y => new CharacterDto { Id = y.Id, Name = y.Name }).ToList(),
                    Areas = x.Areas.Select(y => new AreaDto { Id = y.Id, Name = y.Name }).ToList(),
                    Music = x.Music.Select(y => new MusicDto { Id = y.Id, Name = y.Name, Link = y.Link }).ToList(),
                    Script = new ScriptDto
                    {
                        Id = x.Script.Id,
                        SceneName = x.Script.SceneName,
                        GameName = x.Script.GameName,
                        Lines = x.Script.Lines.Select(y => new ScriptLineDto { Id = y.Id, Order = y.Order, Character = y.Character, Line = y.Line }).OrderBy(y => y.Order).ToList()
                    },
                    Notes = x.Notes
                });
        }
    }
}
