﻿using WayfinderProjectAPI.Data.DTOs;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Data
{
    public static class Extensions
    {
        #region Memory Archive
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
                    //Script = new ScriptDto
                    //{
                    //    Id = x.Script.Id,
                    //    SceneName = x.Script.SceneName,
                    //    GameName = x.Script.GameName,
                    //    Lines = x.Script.Lines.Select(y => new ScriptLineDto { Id = y.Id, Order = y.Order, Character = y.Character, Line = y.Line }).OrderBy(y => y.Order).ToList()
                    //},
                    Notes = x.Notes
                });
        }

        public static ScriptDto ToDto(this Script script)
        {
            return new ScriptDto
            {
                Id = script.Id,
                SceneName = script.SceneName,
                GameName = script.GameName,
                Lines = script.Lines.Select(y => new ScriptLineDto { Id = y.Id, Order = y.Order, Character = y.Character, Line = y.Line }).OrderBy(y => y.Order).ToList()
            };
        }
        #endregion Memory Archive

        #region Jiminy Journal
        public static IQueryable<JournalEntryDto> ToDto(this IQueryable<JournalEntry> entries)
        {
            return entries
                .Select(x => new JournalEntryDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    AdditionalInformation = x.AdditionalInformation,
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name },
                    Characters = x.Characters.Select(y => new CharacterDto { Id = y.Id, Name = y.Name }).ToList(),
                    Worlds = x.Worlds.Select(y => new WorldDto { Id = y.Id, Name = y.Name }).ToList(),
                    Category = x.Category
                });
        }
        #endregion Jiminy Journal

        #region Moogle Shop
        public static IQueryable<RecipeDto> ToDto(this IQueryable<Recipe> recipes)
        {
            return recipes
                .Select(x => new RecipeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                    UnlockConditionDescription = x.UnlockConditionDescription,
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name },
                    RecipeMaterials = x.RecipeMaterials.Select(x => new RecipeMaterialDto
                    {
                        Id = x.Id,
                        Amount = x.Amount,
                        Inventory = new InventoryDto
                        {
                            Id = x.Inventory.Id,
                            Name = x.Inventory.Name,
                            Category = x.Inventory.Category,
                            Cost = x.Inventory.Cost,
                            Currency = x.Inventory.Currency,
                            Description = x.Inventory.Description,
                            AdditionalInformation = x.Inventory.AdditionalInformation,
                            Game = new GameDto { Id = x.Inventory.Game.Id, Name = x.Inventory.Game.Name },
                            EnemyDrops = x.Inventory.EnemyDrops.Select(y => new EnemyDropDto
                            {
                                Id = y.Id,
                                DropRate = y.DropRate,
                                AdditionalInformation = y.AdditionalInformation,
                                CharacterLocation = new CharacterLocationDto
                                {
                                    Id = y.CharacterLocation.Id,
                                    Game = new GameDto { Id = y.CharacterLocation.Game.Id, Name = y.CharacterLocation.Game.Name },
                                    Character = new CharacterDto { Id = y.CharacterLocation.Character.Id, Name = y.CharacterLocation.Character.Name },
                                    World = new WorldDto { Id = y.CharacterLocation.World.Id, Name = y.CharacterLocation.World.Name },
                                    Areas = y.CharacterLocation.Areas.Select(area => new AreaDto { Id = area.Id, Name = area.Name }).ToList()
                                }
                            }).ToList()
                        }
                    }).ToList()
                });
        }

        public static IQueryable<InventoryDto> ToDto(this IQueryable<Inventory> inventory)
        {
            return inventory
                .Select(x => new InventoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                    Cost = x.Cost,
                    Currency = x.Currency,
                    Description = x.Description,
                    AdditionalInformation = x.AdditionalInformation,
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name },
                    EnemyDrops = x.EnemyDrops.Select(y => new EnemyDropDto
                    {
                        Id = y.Id,
                        DropRate = y.DropRate,
                        AdditionalInformation = y.AdditionalInformation,
                        CharacterLocation = new CharacterLocationDto
                        {
                            Id = y.CharacterLocation.Id,
                            Game = new GameDto { Id = y.CharacterLocation.Game.Id, Name = y.CharacterLocation.Game.Name },
                            Character = new CharacterDto { Id = y.CharacterLocation.Character.Id, Name = y.CharacterLocation.Character.Name },
                            World = new WorldDto { Id = y.CharacterLocation.World.Id, Name = y.CharacterLocation.World.Name },
                            Areas = y.CharacterLocation.Areas.Select(area => new AreaDto { Id = area.Id, Name = area.Name }).ToList()
                        }
                    }).ToList()
                });
        }
        #endregion Moogle Shop
    }
}
