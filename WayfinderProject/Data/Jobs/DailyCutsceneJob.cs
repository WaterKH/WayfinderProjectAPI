﻿using MySqlConnector;
using Quartz;
using Tweetinvi;
using Tweetinvi.Parameters;
using WayfinderProject.Data.Models;

namespace WayfinderProject.Data.Jobs
{
    [DisallowConcurrentExecution]
    public class DailyCutsceneJob : IJob
    {
        private readonly ILogger<DailyCutsceneJob> _logger;

        public DailyCutsceneJob(ILogger<DailyCutsceneJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var connectionString = Environment.GetEnvironmentVariable("DailyCutsceneConnectionString");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            string countQuery = "SELECT COUNT(*) FROM wayfinderprojectdb.MA_Scene";

            MySqlCommand countCommand = new MySqlCommand(countQuery, connection);
            int count = Convert.ToInt32(countCommand.ExecuteScalar());

            DateTime now = DateTime.Now;
            Random random = new Random((int)now.Ticks);

            string dateCode = now.ToString("yyyyMMdd");
            int randomSceneId = random.Next(0, count);

            // Insert a row to a table (probably yyyymmdd as key, and the random scene id we choose)


            string selectQuery = $"SELECT * FROM wayfinderprojectdb.DailyCutscenes WHERE DateCode = {dateCode}";

            MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
            MySqlDataReader selectResult = selectCommand.ExecuteReader();

            DailyCutscene dailyCutscene = null;
            while (selectResult.Read())
            {
                dailyCutscene = new DailyCutscene
                {
                    Id = (int)selectResult["Id"],
                    DateCode = (string)selectResult["DateCode"],
                    SceneId = (int)selectResult["SceneId"]
                };

                break;
            }
            selectResult.Close();

            if (dailyCutscene == null)
            {
                string insertQuery = $"INSERT INTO wayfinderprojectdb.DailyCutscenes (DateCode, SceneId) VALUES ({dateCode}, {randomSceneId})";

                MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                insertCommand.ExecuteReader();
            }

            // Send out daily tweet
            string consumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey");
            string consumerSecret = Environment.GetEnvironmentVariable("TwitterConsumerSecret");
            string accessKey = Environment.GetEnvironmentVariable("TwitterAccessKey");
            string accessSecret = Environment.GetEnvironmentVariable("TwitterAccessSecret");

            TwitterClient userClient = new TwitterClient(consumerKey, consumerSecret, accessKey, accessSecret);

            // Get scene of the day
            string sceneCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.MA_Scene 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = MA_Scene.GameId 
                                    INNER JOIN wayfinderprojectdb.Script ON Script.SceneName = MA_Scene.Name
                                    INNER JOIN wayfinderprojectdb.ScriptLine ON Script.Id = ScriptLine.ScriptId
                                    WHERE MA_Scene.Id = {dailyCutscene.SceneId} AND ScriptLine.Line <> 'None' AND LENGTH(ScriptLine.Line) < 278";

            MySqlCommand sceneCountCommand = new MySqlCommand(sceneCountQuery, connection);
            int sceneCount = Convert.ToInt32(sceneCountCommand.ExecuteScalar());

            string sceneQuery = $@"SELECT MA_Scene.Name AS Name, MA_Scene.Link AS Link, Games.Name AS GameName, ScriptLine.Line AS Line FROM wayfinderprojectdb.MA_Scene 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = MA_Scene.GameId 
                                    INNER JOIN wayfinderprojectdb.Script ON Script.SceneName = MA_Scene.Name
                                    INNER JOIN wayfinderprojectdb.ScriptLine ON Script.Id = ScriptLine.ScriptId
                                    WHERE MA_Scene.Id = {dailyCutscene.SceneId} AND ScriptLine.Line <> 'None' AND LENGTH(ScriptLine.Line) < 278";

            MySqlCommand sceneCommand = new MySqlCommand(sceneQuery, connection);
            MySqlDataReader sceneResult = sceneCommand.ExecuteReader();

            int randomSceneDialogue = random.Next(0, sceneCount);
            int counter = 0;
            string tweetText = string.Empty;
            string subTweetText = string.Empty;
            while (sceneResult.Read())
            {
                if (counter != randomSceneDialogue)
                {
                    counter += 1;
                    continue;
                }

                string line = (string)sceneResult["Line"];
                string name = (string)sceneResult["Name"];
                string gameName = (string)sceneResult["GameName"];
                string link = (string)sceneResult["Link"];

                // Assemble tweet and subtweet
                tweetText = $"\"{line}\"";
                subTweetText = $"~\"{name}\" from {gameName}\r\n{link}";

                break;
            }
            sceneResult.Close();

            // Tweet the results
            var tweet = userClient.Tweets.PublishTweetAsync(tweetText).Result;

            PublishTweetParameters parameters = new PublishTweetParameters
            {
                InReplyToTweetId = tweet.Id,
                Text = subTweetText
            };
            var subTweet = userClient.Tweets.PublishTweetAsync(parameters).Result;

            connection.Close();

            return Task.CompletedTask;
        }
    }
}