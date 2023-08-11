using WayfinderProject.Data.Models;
using WayfinderProjectAPI.Data.DTOs;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Data
{
    public static class Extensions
    {
        public static bool IsAdmin(this WayfinderProjectUser user)
        {
            List<string> allowedUsers = new() { "regularpatyt@gmail.com", "waterkh@outlook.com" };

            return allowedUsers.Contains(user.UserName?.ToLower() ?? "");
        }

        #region Memory Archive
        public static IQueryable<SceneDto> ToDto(this IQueryable<Scene> scenes)
        {
            return scenes
                .Select(x => new SceneDto
                {
                    Id = x.Id,
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name, Order = x.Game.Order },
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
                    Notes = x.Notes,
                    State = x.State
                });
        }

        public static SceneDto ToDto(this Scene scene)
        {
            return new SceneDto
            {
                Id = scene.Id,
                Game = new GameDto { Id = scene.Game.Id, Name = scene.Game.Name, Order = scene.Game.Order },
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
                Notes = scene.Notes,
                State = scene.State
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

        public static IQueryable<InteractionDto> ToDto(this IQueryable<Interaction> interactions)
        {
            return interactions
                .Select(x => new InteractionDto
                {
                    Id = x.Id,
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name, Order = x.Game.Order },
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
                    Notes = x.Notes,
                    State = x.State
                });
        }

        public static InteractionDto ToDto(this Interaction interaction)
        {
            return new InteractionDto
            {
                Id = interaction.Id,
                Game = new GameDto { Id = interaction.Game.Id, Name = interaction.Game.Name, Order = interaction.Game.Order },
                Name = interaction.Name,
                Link = interaction.Link,
                Worlds = interaction.Worlds.Select(y => new WorldDto { Id = y.Id, Name = y.Name }).ToList(),
                Characters = interaction.Characters.Select(y => new CharacterDto { Id = y.Id, Name = y.Name }).ToList(),
                Areas = interaction.Areas.Select(y => new AreaDto { Id = y.Id, Name = y.Name }).ToList(),
                Music = interaction.Music.Select(y => new MusicDto { Id = y.Id, Name = y.Name, Link = y.Link }).ToList(),
                //Script = new ScriptDto
                //{
                //    Id = interaction.Script.Id,
                //    SceneName = interaction.Script.SceneName,
                //    GameName = interaction.Script.GameName,
                //    Lines = interaction.Script.Lines.Select(y => new ScriptLineDto { Id = y.Id, Order = y.Order, Character = y.Character, Line = y.Line }).OrderBy(y => y.Order).ToList()
                //},
                Notes = interaction.Notes,
                State = interaction.State
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
                    Games = x.Games.Select(y => new GameDto { Id = y.Id, Name = y.Name, Order = y.Order }).ToList(),
                    Participants = x.Participants.Select(y => new PersonDto { Id = y.Id, Name = y.Name, Description = y.Description, Link = y.Link }).ToList(),
                    Provider = new ProviderDto { Id = x.Provider.Id, Name = x.Provider.Name, Description = x.Provider.Description, Link = x.Provider.Link },
                    Translator = new PersonDto { Id = x.Translator.Id, Name = x.Translator.Name, Description = x.Translator.Description, Link = x.Translator.Link },
                    State = x.State
                });
        }

        public static InterviewDto ToDto(this Interview interview)
        {
            return new InterviewDto
            {
                Id = interview.Id,
                Name = interview.Name,
                Link = interview.Link,
                ReleaseDate = interview.ReleaseDate,
                AdditionalLink = interview.AdditionalLink,
                Games = interview.Games.Select(y => new GameDto { Id = y.Id, Name = y.Name, Order = y.Order }).ToList(),
                Participants = interview.Participants.Select(y => new PersonDto { Id = y.Id, Name = y.Name, Description = y.Description, Link = y.Link }).ToList(),
                Provider = new ProviderDto { Id = interview.Provider.Id, Name = interview.Provider.Name, Description = interview.Provider.Description, Link = interview.Provider.Link },
                Translator = new PersonDto { Id = interview.Translator.Id, Name = interview.Translator.Name, Description = interview.Translator.Description, Link = interview.Translator.Link },
                State = interview.State
            };
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
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name, Order = x.Game.Order },
                    Characters = x.Characters.Select(y => new CharacterDto { Id = y.Id, Name = y.Name }).ToList(),
                    Worlds = x.Worlds.Select(y => new WorldDto { Id = y.Id, Name = y.Name }).ToList(),
                    Category = x.Category,
                    State = x.State
                });
        }

        public static JournalEntryDto ToDto(this JournalEntry entry)
        {
            return new JournalEntryDto
            {
                Id = entry.Id,
                Game = new GameDto { Id = entry.Game.Id, Name = entry.Game.Name, Order = entry.Game.Order },
                Title = entry.Title,
                Description = entry.Description,
                Worlds = entry.Worlds.Select(y => new WorldDto { Id = y.Id, Name = y.Name }).ToList(),
                Characters = entry.Characters.Select(y => new CharacterDto { Id = y.Id, Name = y.Name }).ToList(),
                Category = entry.Category,
                AdditionalInformation = entry.AdditionalInformation,
                State = entry.State
            };
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
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name, Order = x.Game.Order },
                    RecipeMaterials = x.RecipeMaterials.Select(y => new RecipeMaterialDto
                    {
                        Id = y.Id,
                        Amount = y.Amount,
                        Inventory = new InventoryDto
                        {
                            Id = y.Inventory.Id,
                            Name = y.Inventory.Name,
                            Category = y.Inventory.Category,
                            Cost = y.Inventory.Cost,
                            Currency = y.Inventory.Currency,
                            Description = y.Inventory.Description,
                            AdditionalInformation = y.Inventory.AdditionalInformation,
                            Game = new GameDto { Id = y.Inventory.Game.Id, Name = y.Inventory.Game.Name, Order = y.Inventory.Game.Order },
                            EnemyDrops = y.Inventory.EnemyDrops.Select(z => new EnemyDropDto
                            {
                                Id = z.Id,
                                DropRate = z.DropRate,
                                AdditionalInformation = z.AdditionalInformation,
                                CharacterLocation = new CharacterLocationDto
                                {
                                    Id = z.CharacterLocation.Id,
                                    Game = new GameDto { Id = z.CharacterLocation.Game.Id, Name = z.CharacterLocation.Game.Name, Order = z.CharacterLocation.Game.Order },
                                    Character = new CharacterDto { Id = z.CharacterLocation.Character.Id, Name = z.CharacterLocation.Character.Name },
                                    World = new WorldDto { Id = z.CharacterLocation.World.Id, Name = z.CharacterLocation.World.Name },
                                    Areas = z.CharacterLocation.Areas.Select(area => new AreaDto { Id = area.Id, Name = area.Name }).ToList()
                                }
                            }).ToList()
                        }
                    }).ToList(),
                    State = x.State
                });
        }

        public static RecipeDto ToDto(this Recipe recipe)
        {
            return new RecipeDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Category = recipe.Category,
                UnlockConditionDescription = recipe.UnlockConditionDescription,
                Game = new GameDto { Id = recipe.Game.Id, Name = recipe.Game.Name, Order = recipe.Game.Order },
                RecipeMaterials = recipe.RecipeMaterials.Select(x => new RecipeMaterialDto
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
                        Game = new GameDto { Id = x.Inventory.Game.Id, Name = x.Inventory.Game.Name, Order = x.Inventory.Game.Order },
                        EnemyDrops = x.Inventory.EnemyDrops.Select(y => new EnemyDropDto
                        {
                            Id = y.Id,
                            DropRate = y.DropRate,
                            AdditionalInformation = y.AdditionalInformation,
                            CharacterLocation = new CharacterLocationDto
                            {
                                Id = y.CharacterLocation.Id,
                                Game = new GameDto { Id = y.CharacterLocation.Game.Id, Name = y.CharacterLocation.Game.Name, Order = y.CharacterLocation.Game.Order },
                                Character = new CharacterDto { Id = y.CharacterLocation.Character.Id, Name = y.CharacterLocation.Character.Name },
                                World = new WorldDto { Id = y.CharacterLocation.World.Id, Name = y.CharacterLocation.World.Name },
                                Areas = y.CharacterLocation.Areas.Select(area => new AreaDto { Id = area.Id, Name = area.Name }).ToList()
                            }
                        }).ToList()
                    }
                }).ToList(),
                State = recipe.State
            };
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
                    Game = new GameDto { Id = x.Game.Id, Name = x.Game.Name, Order = x.Game.Order },
                    EnemyDrops = x.EnemyDrops.Select(y => new EnemyDropDto
                    {
                        Id = y.Id,
                        DropRate = y.DropRate,
                        AdditionalInformation = y.AdditionalInformation,
                        CharacterLocation = new CharacterLocationDto
                        {
                            Id = y.CharacterLocation.Id,
                            Game = new GameDto { Id = y.CharacterLocation.Game.Id, Name = y.CharacterLocation.Game.Name, Order = y.CharacterLocation.Game.Order },
                            Character = new CharacterDto { Id = y.CharacterLocation.Character.Id, Name = y.CharacterLocation.Character.Name },
                            World = new WorldDto { Id = y.CharacterLocation.World.Id, Name = y.CharacterLocation.World.Name },
                            Areas = y.CharacterLocation.Areas.Select(area => new AreaDto { Id = area.Id, Name = area.Name }).ToList()
                        }
                    }).ToList(),
                    State = x.State
                });
        }

        public static InventoryDto ToDto(this Inventory inventory)
        {
            return new InventoryDto
            {
                Id = inventory.Id,
                Game = new GameDto { Id = inventory.Game.Id, Name = inventory.Game.Name, Order = inventory.Game.Order },
                Name = inventory.Name,
                Description = inventory.Description,
                Cost = inventory.Cost,
                Currency = inventory.Currency,
                Category = inventory.Category,
                AdditionalInformation = inventory.AdditionalInformation,
                EnemyDrops = inventory.EnemyDrops.Select(y => new EnemyDropDto
                {
                    Id = y.Id,
                    DropRate = y.DropRate,
                    AdditionalInformation = y.AdditionalInformation,
                    CharacterLocation = new CharacterLocationDto
                    {
                        Id = y.CharacterLocation.Id,
                        Game = new GameDto { Id = y.CharacterLocation.Game.Id, Name = y.CharacterLocation.Game.Name, Order = y.CharacterLocation.Game.Order },
                        Character = new CharacterDto { Id = y.CharacterLocation.Character.Id, Name = y.CharacterLocation.Character.Name },
                        World = new WorldDto { Id = y.CharacterLocation.World.Id, Name = y.CharacterLocation.World.Name },
                        Areas = y.CharacterLocation.Areas.Select(area => new AreaDto { Id = area.Id, Name = area.Name }).ToList()
                    }
                }).ToList(),
                State = inventory.State
            };
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
                IncludeAlias = settings.IncludeAlias,
                ResultsDisplay = settings.ResultsDisplay
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
