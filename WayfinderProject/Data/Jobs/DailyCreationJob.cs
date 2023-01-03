using MySqlConnector;
using Quartz;
using WayfinderProject.Data.Models;

namespace WayfinderProject.Data.Jobs
{
    [DisallowConcurrentExecution]
    public class DailyCreationJob : IJob
    {
        private readonly ILogger<DailyCreationJob> _logger;

        public DailyCreationJob(ILogger<DailyCreationJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            DateTime now = DateTime.Now;
            Random random = new Random((int)now.Ticks);

            string dateCode = now.ToString("yyyyMMdd");

            var connectionString = Environment.GetEnvironmentVariable("DailyCutsceneConnectionString");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            #region Cutscene
            string selectSceneQuery = $"SELECT * FROM wayfinderprojectdb.DailyCutscenes WHERE DateCode = {dateCode}";

            MySqlCommand selectSceneCommand = new MySqlCommand(selectSceneQuery, connection);
            MySqlDataReader selectSceneResult = selectSceneCommand.ExecuteReader();

            DailyCutscene? dailyCutscene = null;
            while (selectSceneResult.Read())
            {
                dailyCutscene = new DailyCutscene
                {
                    Id = (int)selectSceneResult["Id"],
                    DateCode = (string)selectSceneResult["DateCode"],
                    SceneId = (int)selectSceneResult["SceneId"]
                };

                break;
            }
            selectSceneResult.Close();

            // Only create a daily tweet if don't already have one
            if (dailyCutscene == null)
            {
                // Get random daily scene
                string sceneCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.MA_Scene 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = MA_Scene.GameId 
                                    INNER JOIN wayfinderprojectdb.Script ON Script.SceneName = MA_Scene.Name
                                    INNER JOIN wayfinderprojectdb.ScriptLine ON Script.Id = ScriptLine.ScriptId
                                    WHERE ScriptLine.Line <> 'None' AND LENGTH(ScriptLine.Line) < 253";

                MySqlCommand sceneCountCommand = new MySqlCommand(sceneCountQuery, connection);
                int sceneCount = Convert.ToInt32(sceneCountCommand.ExecuteScalar());

                string sceneQuery = $@"SELECT MA_Scene.Id AS SceneId, MA_Scene.Name AS Name, MA_Scene.Link AS Link, Games.Name AS GameName, ScriptLine.Line AS Line, ScriptLine.Character AS 'Character'
                                    FROM wayfinderprojectdb.MA_Scene 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = MA_Scene.GameId 
                                    INNER JOIN wayfinderprojectdb.Script ON Script.SceneName = MA_Scene.Name
                                    INNER JOIN wayfinderprojectdb.ScriptLine ON Script.Id = ScriptLine.ScriptId
                                    WHERE ScriptLine.Line <> 'None' AND LENGTH(ScriptLine.Line) < 253";

                MySqlCommand sceneCommand = new MySqlCommand(sceneQuery, connection);
                MySqlDataReader sceneResult = sceneCommand.ExecuteReader();

                int randomSceneDialogue = random.Next(0, sceneCount);
                int randomSceneId = 0;
                int counter = 0;
                while (sceneResult.Read())
                {
                    if (counter != randomSceneDialogue)
                    {
                        counter += 1;
                        continue;
                    }

                    randomSceneId = (int)sceneResult["SceneId"];

                    break;
                }
                sceneResult.Close();

                string insertQuery = $"INSERT INTO wayfinderprojectdb.DailyCutscenes (DateCode, SceneId) VALUES ({dateCode}, {randomSceneId})";

                MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                insertCommand.ExecuteReader();
            }
            #endregion

            #region Journal
            string selectJournalQuery = $"SELECT * FROM wayfinderprojectdb.DailyJournalEntries WHERE DateCode = {dateCode}";

            MySqlCommand selectJournalCommand = new MySqlCommand(selectJournalQuery, connection);
            MySqlDataReader selectJournalResult = selectJournalCommand.ExecuteReader();

            DailyJournalEntry? dailyJournalEntry = null;
            while (selectJournalResult.Read())
            {
                dailyJournalEntry = new DailyJournalEntry
                {
                    Id = (int)selectJournalResult["Id"],
                    DateCode = (string)selectJournalResult["DateCode"],
                    EntryId = (int)selectJournalResult["EntryId"]
                };

                break;
            }
            selectJournalResult.Close();

            // Only create a daily tweet if don't already have one
            if (dailyJournalEntry == null)
            {
                // Get random daily scene
                string journalEntryCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.JJ_Entry 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = JJ_Entry.GameId";

                MySqlCommand journalEntryCountCommand = new MySqlCommand(journalEntryCountQuery, connection);
                int journalEntryCount = Convert.ToInt32(journalEntryCountCommand.ExecuteScalar());

                string journalEntryQuery = $@"SELECT JJ_Entry.Id AS EntryId, JJ_Entry.Title AS Title, JJ_Entry.Description AS Description, JJ_Entry.Category AS Category, Games.Name AS GameName
                                    FROM wayfinderprojectdb.JJ_Entry 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = JJ_Entry.GameId";

                MySqlCommand journalEntryCommand = new MySqlCommand(journalEntryQuery, connection);
                MySqlDataReader journalEntryResult = journalEntryCommand.ExecuteReader();

                int randomEntry = random.Next(0, journalEntryCount);
                int counter = 0;
                int randomEntryId = 0;
                string tweetText = string.Empty;
                string subTweetText = string.Empty;
                while (journalEntryResult.Read())
                {
                    if (counter != randomEntry)
                    {
                        counter += 1;
                        continue;
                    }

                    randomEntryId = (int)journalEntryResult["EntryId"];

                    break;
                }
                journalEntryResult.Close();

                string insertQuery = $"INSERT INTO wayfinderprojectdb.DailyJournalEntries (DateCode, EntryId) VALUES ({dateCode}, {randomEntryId})";

                MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                insertCommand.ExecuteReader();
            }
            #endregion Journal

            #region Shop
            string selectShopQuery = $"SELECT * FROM wayfinderprojectdb.DailyMoogleRecords WHERE DateCode = {dateCode}";

            MySqlCommand selectShopCommand = new MySqlCommand(selectShopQuery, connection);
            MySqlDataReader selectShopResult = selectShopCommand.ExecuteReader();

            DailyMoogleRecord? dailyMoogleRecord = null;
            while (selectShopResult.Read())
            {
                dailyMoogleRecord = new DailyMoogleRecord
                {
                    Id = (int)selectShopResult["Id"],
                    DateCode = (string)selectShopResult["DateCode"],
                    RecordId = (int)selectShopResult["RecordId"]
                };

                break;
            }
            selectShopResult.Close();

            // Only create a daily tweet if don't already have one
            if (dailyMoogleRecord == null)
            {
                // Get random daily scene
                string moogleRecordCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.MS_Inventory";

                MySqlCommand moogleRecordCountCommand = new MySqlCommand(moogleRecordCountQuery, connection);
                int moogleRecordCount = Convert.ToInt32(moogleRecordCountCommand.ExecuteScalar());

                string moogleRecordQuery = $@"SELECT MS_Inventory.Id AS RecordId FROM wayfinderprojectdb.MS_Inventory";

                MySqlCommand moogleRecordCommand = new MySqlCommand(moogleRecordQuery, connection);
                MySqlDataReader moogleRecordResult = moogleRecordCommand.ExecuteReader();

                int randomRecord = random.Next(0, moogleRecordCount);
                int counter = 0;
                int randomRecordId = 0;
                while (moogleRecordResult.Read())
                {
                    if (counter != randomRecord)
                    {
                        counter += 1;
                        continue;
                    }

                    randomRecordId = (int)moogleRecordResult["RecordId"];

                    break;
                }
                moogleRecordResult.Close();

                string insertQuery = $"INSERT INTO wayfinderprojectdb.DailyMoogleRecords (DateCode, RecordId) VALUES ({dateCode}, {randomRecordId})";

                MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                insertCommand.ExecuteReader();

                connection.Close();
            }
            #endregion Shop

            return Task.CompletedTask;
        }
    }
}