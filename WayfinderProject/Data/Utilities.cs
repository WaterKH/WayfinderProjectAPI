using static WayfinderProjectAPI.Data.DatabaseInitializer;

namespace WayfinderProjectAPI.Data
{
    public static class Utilities
    {
        #region Memory Archive
        public static List<SceneObject> ProcessScenes(string[] lines)
        {
            var scenes = new List<SceneObject>();

            SceneObject? scene = null;
            int order = 0;
            foreach (var line in lines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (!line.Contains(": "))
                    {
                        // Title
                        if (scene != null)
                        {
                            scenes.Add(scene);
                        }

                        scene = new SceneObject()
                        {
                            Name = line
                        };

                        order = 0;
                    }
                    else
                    {
                        // Content
                        if (scene == null) continue;

                        var splitLine = line.Split(": ", 2);
                        switch (splitLine[0])
                        {
                            case "Link":
                            case "Clip":
                                scene.Link = splitLine[1];
                                break;
                            case "Characters":
                                scene.Characters = splitLine[1].Split(", ").ToList();
                                break;
                            case "World":
                                scene.Worlds = splitLine[1].Split(", ").ToList();
                                break;
                            case "Area":
                                scene.Areas = splitLine[1].Split(", ").ToList();
                                break;
                            case "Music":
                                scene.Music = splitLine[1].Split(", ").ToList();
                                break;
                            default:
                                // Character Lines
                                var character = splitLine[0];
                                var value = splitLine[1];

                                scene.Dialogue.Add(new LineScriptObject { Character = character.Trim(), Line = value.Trim(), Order = order });
                                ++order;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (scene != null)
                scenes.Add(scene);

            return scenes;
        }

        public static Dictionary<string, List<LineScriptObject>> ProcessScript(string[] lines)
        {
            var dict = new Dictionary<string, List<LineScriptObject>>();
            var order = 0;
            var scene = "";
            try
            {
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains(": "))
                    {
                        var splitLine = line.Split(": ", 2);
                        var scriptLine = new LineScriptObject
                        {
                            Order = order,
                            Character = splitLine[0],
                            Line = splitLine[1]
                        };

                        dict[scene].Add(scriptLine);
                        ++order;
                    }
                    else
                    {
                        scene = line;
                        order = 0;

                        dict.Add(scene, new List<LineScriptObject>());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dict;
        }

        public static InterviewObject ProcessInterview(string[] lines)
        {
            InterviewObject interview = new InterviewObject();
            bool reachedDialogue = false;
            int order = 0;
            foreach (var line in lines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains("Text:"))
                    {
                        reachedDialogue = true;
                        continue;
                    }

                    if (!reachedDialogue)
                    {
                        var splitLine = line.Split(";; ");
                        switch (splitLine[0])
                        {
                            case "Name":
                                interview.Name = splitLine[1];
                                break;
                            case "Release Date":
                                interview.ReleaseDate = DateTime.ParseExact(splitLine[1], "MM/dd/yyyy", null);
                                break;
                            case "Game(s)":
                                interview.GameNames = splitLine[1].Split(", ").ToList();
                                break;
                            case "Provider":
                                interview.Provider = splitLine[1];
                                break;
                            case "Provider Link":
                                interview.Link = splitLine[1];
                                break;
                            case "Translator":
                                interview.Translator = splitLine[1];
                                break;
                            case "Video Link":
                                interview.AdditionalLink = splitLine[1];
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        var splitLine = line.Split(":: ");
                        var participant = splitLine[0];
                        var value = splitLine[1];

                        if (!interview.Participants.Contains(participant)) interview.Participants.Add(participant);

                        interview.Conversation.Add(new InterviewLineObject { Speaker = participant.Trim(), Line = value.Trim(), Order = order });
                        ++order;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return interview;
        }

        public static List<InteractionObject> ProcessInteractions(string[] lines)
        {
            var interactions = new List<InteractionObject>();

            InteractionObject? interaction = null;
            int order = 0;
            foreach (var line in lines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (!line.Contains(": "))
                    {
                        // Title
                        if (interaction != null)
                        {
                            interactions.Add(interaction);
                        }

                        interaction = new InteractionObject()
                        {
                            Title = line
                        };

                        order = 0;
                    }
                    else
                    {
                        // Content
                        if (interaction == null) continue;

                        var splitLine = line.Split(": ", 2);
                        switch (splitLine[0])
                        {
                            case "Link":
                            case "Clip":
                                interaction.Link = splitLine[1];
                                break;
                            case "Characters":
                                interaction.Characters = splitLine[1].Split(", ").ToList();
                                break;
                            case "World":
                                interaction.Worlds = splitLine[1].Split(", ").ToList();
                                break;
                            case "Area":
                                interaction.Areas = splitLine[1].Split(", ").ToList();
                                break;
                            case "Music":
                                interaction.Music = splitLine[1].Split(", ").ToList();
                                break;
                            default:
                                // Character Lines
                                var character = splitLine[0];
                                var value = splitLine[1];

                                interaction.Interactions.Add(new InteractionLineObject { Character = character.Trim(), Line = value.Trim(), Order = order });
                                ++order;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (interaction != null)
                interactions.Add(interaction);

            return interactions;
        }

        public static List<TrailerObject> ProcessTrailers(string[] lines)
        {
            var trailers = new List<TrailerObject>();

            TrailerObject? trailer = null;
            int order = 0;
            foreach (var line in lines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (!line.Contains(": "))
                    {
                        // Title
                        if (trailer != null)
                        {
                            trailers.Add(trailer);
                        }

                        trailer = new TrailerObject()
                        {
                            Title = line
                        };

                        order = 0;
                    }
                    else
                    {
                        // Content
                        if (trailer == null) continue;

                        var splitLine = line.Split(": ", 2);
                        switch (splitLine[0])
                        {
                            case "Link":
                            case "Clip":
                                trailer.Link = splitLine[1];
                                break;
                            case "Characters":
                                trailer.Characters = splitLine[1].Split(", ").ToList();
                                break;
                            case "World":
                                trailer.Worlds = splitLine[1].Split(", ").ToList();
                                break;
                            case "Area":
                                trailer.Areas = splitLine[1].Split(", ").ToList();
                                break;
                            case "Music":
                                trailer.Music = splitLine[1].Split(", ").ToList();
                                break;
                            default:
                                // Character Lines
                                var character = splitLine[0];
                                var value = splitLine[1];

                                trailer.Trailers.Add(new TrailerLineObject { Character = character.Trim(), Line = value.Trim(), Order = order });
                                ++order;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (trailer != null)
                trailers.Add(trailer);

            return trailers;
        }
        #endregion Memory Archive

        #region Jiminy's Journal
        public static List<JJCharacterObject> ProcessCharacter(string[] lines)
        {
            var characters = new List<JJCharacterObject>();

            try
            {
                JJCharacterObject? currentCharacter = null;
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains("::"))
                    {
                        if (currentCharacter != null)
                        {
                            characters.Add(currentCharacter);
                        }

                        currentCharacter = new JJCharacterObject()
                        {
                            Title = line.Replace("::", ""),
                            Characters = new List<string> { line.Replace("::", "") }
                        };
                    }
                    else
                    {
                        if (currentCharacter == null) continue;

                        //currentCharacter.Description += line + " ";
                        if (string.IsNullOrEmpty(currentCharacter.Description))
                        {
                            currentCharacter.Description = line;
                        }
                        else
                        {
                            currentCharacter.AdditionalInformation = line;
                        }
                    }
                }

                if (currentCharacter != null)
                {
                    characters.Add(currentCharacter);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return characters;
        }

        public static List<JJCharacterObject> ProcessStory(string[] lines)
        {
            var story = new List<JJCharacterObject>();

            try
            {
                JJCharacterObject? currentStory = null;
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains("::"))
                    {
                        if (currentStory != null)
                        {
                            story.Add(currentStory);
                        }

                        currentStory = new JJCharacterObject()
                        {
                            Title = line.Replace("::", "")
                        };
                    }
                    else
                    {
                        if (currentStory == null) continue;

                        var splitLine = line.Split(": ");
                        switch (splitLine[0])
                        {
                            case "Worlds":
                                currentStory.Worlds = splitLine[1].Split(", ").ToList();
                                break;
                            case "Characters":
                                currentStory.Characters = splitLine[1].Split(", ").ToList();
                                break;
                            case "Additional Information":
                                currentStory.AdditionalInformation = splitLine[1];
                                break;
                            default:
                                currentStory.Description += line + " \r\n";
                                break;
                        }
                    }
                }

                if (currentStory != null)
                {
                    story.Add(currentStory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return story;
        }

        public static List<JJCharacterObject> ProcessReports(string[] lines)
        {
            var report = new List<JJCharacterObject>();

            try
            {
                JJCharacterObject? currentReport = null;
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains("::"))
                    {
                        if (currentReport != null)
                        {
                            report.Add(currentReport);
                        }

                        currentReport = new JJCharacterObject()
                        {
                            Title = line.Replace("::", "")
                        };
                    }
                    else
                    {
                        if (currentReport == null) continue;

                        var splitLine = line.Split(": ");
                        switch (splitLine[0])
                        {
                            case "Worlds":
                                currentReport.Worlds = splitLine[1].Split(", ").ToList();
                                break;
                            case "Characters":
                                currentReport.Characters = splitLine[1].Split(", ").ToList();
                                break;
                            case "Additional Information":
                                currentReport.AdditionalInformation = splitLine[1];
                                break;
                            default:
                                currentReport.Description += line + " \r\n";
                                break;
                        }
                    }
                }

                if (currentReport != null)
                {
                    report.Add(currentReport);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return report;
        }

        public static List<JJCharacterObject> ProcessEnemy(string[] lines)
        {
            var enemies = new List<JJCharacterObject>();

            try
            {
                JJCharacterObject? currentEnemy = null;
                List<string> worlds = new List<string>();
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains("::"))
                    {
                        if (currentEnemy != null)
                        {
                            if (currentEnemy.Characters.Count == 0)
                                currentEnemy.Characters = new List<string> { currentEnemy.Title + "(s)" };

                            currentEnemy.Worlds = worlds.ToList();
                            worlds.Clear();

                            enemies.Add(currentEnemy);
                        }

                        currentEnemy = new JJCharacterObject()
                        {
                            Title = line.Replace("::", "")
                        };
                    }
                    else
                    {
                        var splitLine = line.Split(": ");

                        if (splitLine.Length == 2 && splitLine[1].Length == 0)
                        {
                            worlds.Add(splitLine[0]);
                        }
                        else
                        {
                            if (currentEnemy == null) continue;

                            switch (splitLine[0])
                            {
                                case "Worlds":
                                    currentEnemy.Worlds = splitLine[1].Split(", ").ToList();
                                    break;
                                case "Characters":
                                    currentEnemy.Characters = splitLine[1].Split(", ").ToList();
                                    break;
                                case "Additional Information":
                                case "TIP":
                                    currentEnemy.AdditionalInformation = splitLine[1];
                                    break;
                                default:
                                    currentEnemy.Description += line + " \r\n";
                                    break;
                            }
                        }
                    }
                }

                if (currentEnemy != null)
                {
                    enemies.Add(currentEnemy);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return enemies;
        }
        #endregion Jiminy's Journal

        #region Moogle Shop
        public static List<MSInventoryObject> ProcessInventory(string[] lines)
        {
            var inventory = new List<MSInventoryObject>();

            MSInventoryObject? currentItem = null;
            foreach (var line in lines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains("::"))
                    {
                        if (currentItem != null)
                        {
                            inventory.Add(currentItem);
                        }

                        currentItem = new MSInventoryObject()
                        {
                            Name = line.Replace("::", ""),
                            //Category = category
                        };
                    }
                    //else if (!line.Contains(": "))
                    //{
                    //    if (string.IsNullOrEmpty(currentItem.Description))
                    //    {
                    //        currentItem.Description = line;
                    //    }
                    //    else
                    //    {
                    //        currentItem.AdditionalInformation = line;
                    //    }
                    //}
                    else
                    {
                        if (currentItem == null) continue;

                        var splitLine = line.Split(": ");
                        switch (splitLine[0])
                        {
                            case "Cost":
                                currentItem.Cost = int.Parse(splitLine[1]);
                                break;
                            case "Currency":
                                currentItem.Currency = splitLine[1];
                                break;
                            case "Category":
                                currentItem.Category = splitLine[1];
                                break;
                            case "Description":
                                currentItem.Description = splitLine[1];
                                break;
                            case "Additional Information":
                                currentItem.AdditionalInformation = splitLine[1];
                                break;
                            default:
                                var subSplitLines = splitLine[1].Split("; ");
                                var enemyDrop = new MSEnemyDropObject
                                {
                                    EnemyName = splitLine[0],
                                    DropRate = float.Parse(subSplitLines[0]),
                                    AdditionalInformation = subSplitLines.Length > 1 ? subSplitLines[1] : string.Empty
                                };

                                currentItem.EnemyDrops.Add(enemyDrop);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (currentItem != null)
            {
                inventory.Add(currentItem);
            }

            return inventory;
        }

        public static List<MSRecipeObject> ProcessRecipe(string[] lines)
        {
            var recipes = new List<MSRecipeObject>();

            MSRecipeObject? currentRecipe = null;
            foreach (var line in lines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains("::"))
                    {
                        if (currentRecipe != null)
                        {
                            recipes.Add(currentRecipe);
                        }

                        currentRecipe = new MSRecipeObject()
                        {
                            Name = line.Replace("::", "")
                        };
                    }
                    else
                    {
                        if (currentRecipe == null) continue;

                        var splitLine = line.Split(" - ");
                        switch (splitLine[0])
                        {
                            case "Unlock Condition":
                                currentRecipe.UnlockConditionDescription = splitLine[1];
                                break;
                            case "Category":
                                currentRecipe.Category = splitLine[1];
                                break;
                            default:
                                currentRecipe.MaterialsNeeded.Add(splitLine[0], int.Parse(splitLine[1]));
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (currentRecipe != null)
            {
                recipes.Add(currentRecipe);
            }

            return recipes;
        }

        public static List<MSMiscEnemyObject> ProcessEnemyLocation(string[] lines)
        {
            var enemies = new List<MSMiscEnemyObject>();

            MSMiscEnemyObject? currentEnemy = null;
            foreach (var line in lines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Contains("::"))
                    {
                        if (currentEnemy != null)
                        {
                            enemies.Add(currentEnemy);
                        }

                        currentEnemy = new MSMiscEnemyObject()
                        {
                            CharacterName = line.Replace("::", ""),
                            WorldWithAreas = new Dictionary<string, List<string>>()
                        };
                    }
                    else
                    {
                        if (currentEnemy == null) continue;

                        var splitLine = line.Split(": ");
                        var world = splitLine[0];
                        var areas = new List<string>();

                        if (splitLine.Length > 1)
                            areas = splitLine[1].Split("; ").ToList();

                        currentEnemy.WorldWithAreas.Add(world, areas);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (currentEnemy != null)
            {
                enemies.Add(currentEnemy);
            }

            return enemies;
        }
        #endregion Moogle Shop
    }
}
