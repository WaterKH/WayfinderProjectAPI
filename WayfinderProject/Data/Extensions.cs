using WayfinderProjectAPI.Data.DTOs;
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

        public static SceneDto ToDto(this Scene scene)
        {
            return new SceneDto
            {
                Id = scene.Id,
                Game = new GameDto { Id = scene.Game.Id, Name = scene.Game.Name },
                Name = scene.Name,
                Link = scene.Link,
                Worlds = scene.Worlds.Select(y => new WorldDto { Id = y.Id, Name = y.Name }).ToList(),
                Characters = scene.Characters.Select(y => new CharacterDto { Id = y.Id, Name = y.Name }).ToList(),
                Areas = scene.Areas.Select(y => new AreaDto { Id = y.Id, Name = y.Name }).ToList(),
                Music = scene.Music.Select(y => new MusicDto { Id = y.Id, Name = y.Name, Link = y.Link }).ToList(),
                //Script = new ScriptDto
                //{
                //    Id = scene.Script.Id,
                //    SceneName = scene.Script.SceneName,
                //    GameName = scene.Script.GameName,
                //    Lines = scene.Script.Lines.Select(y => new ScriptLineDto { Id = y.Id, Order = y.Order, Character = y.Character, Line = y.Line }).OrderBy(y => y.Order).ToList()
                //},
                Notes = scene.Notes
            };
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

        public static IQueryable<InterviewDto> ToDto(this IQueryable<Interview> interviews)
        {
            return interviews
                .Select(x => new InterviewDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Link = x.Link,
                    ReleaseDate = x.ReleaseDate,
                    AdditionalLink = x.AdditionalLink,
                    Games = x.Games.Select(y => new GameDto { Id = y.Id, Name = y.Name }).ToList(),
                    Participants = x.Participants.Select(y => new PersonDto { Id = y.Id, Name = y.Name, Description = y.Description, Link = y.Link }).ToList(),
                    Provider = new ProviderDto { Id = x.Provider.Id, Name = x.Provider.Name, Description = x.Provider.Description, Link = x.Provider.Link },
                    Translator = new PersonDto { Id = x.Translator.Id, Name = x.Translator.Name, Description = x.Translator.Description, Link = x.Translator.Link }
                });
        }

        public static IEnumerable<InterviewLineDto> ToDto(this List<InterviewLine> conversation)
        {
            return conversation.Select(x => new InterviewLineDto
            {
                Id = x.Id,
                Order = x.Order,
                Speaker = x.Speaker,
                Line = x.Line
            });
        }

        public static IQueryable<AliasDto> ToDto(this IQueryable<Alias> aliases)
        {
            return aliases
                .Select(x => new AliasDto
                {
                    Id = x.Id,
                    Original = x.Original,
                    AppearAs = x.AppearAs
                });
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

        #region Search History And Settings
        public static IQueryable<SearchHistoryDto> ToDto(this IQueryable<SearchHistory> history)
        {
            return history
                .Select(x => new SearchHistoryDto
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    CreatedDate = x.CreatedDate,
                    Type = x.Type,
                    Category = x.Category,
                    TextSearch = x.TextSearch,
                    SpecificSearch = x.SpecificSearch,
                    Param1Search = x.Param1Search,
                    Param2Search = x.Param2Search,
                    Param3Search = x.Param3Search,
                    Param4Search = x.Param4Search,
                    Param5Search = x.Param5Search,
                    Param6Search = x.Param6Search,
                    Param7Search = x.Param7Search,
                });
        }

        public static IQueryable<SearchHistoryDto> ToDto(this IQueryable<SearchHistory> history, string type)
        {
            return history.Where(x => x.Type == type)
                .Select(x => new SearchHistoryDto
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    CreatedDate = x.CreatedDate,
                    Type = x.Type,
                    Category = x.Category,
                    TextSearch = x.TextSearch,
                    SpecificSearch = x.SpecificSearch,
                    Param1Search = x.Param1Search,
                    Param2Search = x.Param2Search,
                    Param3Search = x.Param3Search,
                    Param4Search = x.Param4Search,
                    Param5Search = x.Param5Search,
                    Param6Search = x.Param6Search,
                    Param7Search = x.Param7Search,
                });
        }

        public static SearchSettingsDto ToDto(this SearchSettings settings)
        {
            return new SearchSettingsDto
            {
                Id = settings.Id,
                AccountId = settings.AccountId,
                AutoSearch = settings.AutoSearch,
                AutoExpandFirstResult = settings.AutoExpandFirstResult,
                MainSearchEverything = settings.MainSearchEverything,
                TrackHistory = settings.TrackHistory,
                FavouriteSearch = settings.FavouriteSearch,
                ProjectSearch = settings.ProjectSearch,
                IncludeAlias = settings.IncludeAlias
            };
        }
        #endregion Search History And Settings

        #region Projects And Favorites
        public static IQueryable<ProjectDto> ToDto(this IQueryable<Project> projects)
        {
            return projects
                .Select(x => new ProjectDto
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    Name = x.Name,
                    Description = x.Description,
                    CreatedDate = x.CreatedDate,
                    ProjectRecords = x.ProjectRecords.Select(y => new ProjectRecordDto
                    {
                        Id = y.Id,
                        CreatedDate = y.CreatedDate,
                        Type = y.Type,
                        Category = y.Category,
                        SpecificRecordId = y.SpecificRecordId,
                        Notes = y.Notes
                    }).ToList()
                });
        }

        public static IQueryable<FavoriteDto> ToDto(this IQueryable<Favorite> favorites)
        {
            return favorites
                .Select(x => new FavoriteDto
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    CreatedDate = x.CreatedDate,
                    Type = x.Type,
                    Category = x.Category,
                    SpecificRecordId = x.SpecificRecordId
                });
        }
        #endregion Projects And Favorites
    }
}
