using MySqlConnector;
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
            DateTime now = DateTime.Now;
            Random random = new Random((int)now.Ticks);

            string dateCode = now.ToString("yyyyMMdd");

            var connectionString = Environment.GetEnvironmentVariable("DailyCutsceneConnectionString");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            string selectQuery = $"SELECT * FROM wayfinderprojectdb.DailyCutscenes WHERE DateCode = {dateCode}";

            MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
            MySqlDataReader selectResult = selectCommand.ExecuteReader();

            DailyCutscene? dailyCutscene = null;
            while (selectResult.Read())
            {
                dailyCutscene = new DailyCutscene
                {
                    Id = (int)selectResult["Id"],
                    DateCode = (string)selectResult["DateCode"],
                    SceneId = (int)selectResult["SceneId"],
                    HasTweeted = (bool)selectResult["HasTweeted"]
                };

                break;
            }
            selectResult.Close();

            // Only create a daily tweet if don't already have one
            if (dailyCutscene != null && !dailyCutscene.HasTweeted)
            {
                // Get random daily scene
                string sceneCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.MA_Scene 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = MA_Scene.GameId 
                                    INNER JOIN wayfinderprojectdb.Script ON Script.SceneName = MA_Scene.Name
                                    INNER JOIN wayfinderprojectdb.ScriptLine ON Script.Id = ScriptLine.ScriptId
                                    WHERE MA_Scene.Id = {dailyCutscene.SceneId}";

                MySqlCommand sceneCountCommand = new MySqlCommand(sceneCountQuery, connection);
                int sceneCount = Convert.ToInt32(sceneCountCommand.ExecuteScalar());

                string sceneQuery = $@"SELECT MA_Scene.Id AS SceneId, MA_Scene.Name AS Name, MA_Scene.Link AS Link, Games.Name AS GameName, ScriptLine.Line AS Line, ScriptLine.Character AS 'Character'
                                    FROM wayfinderprojectdb.MA_Scene 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = MA_Scene.GameId 
                                    INNER JOIN wayfinderprojectdb.Script ON Script.SceneName = MA_Scene.Name
                                    INNER JOIN wayfinderprojectdb.ScriptLine ON Script.Id = ScriptLine.ScriptId
                                    WHERE MA_Scene.Id = {dailyCutscene.SceneId}";

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
                    string character = (string)sceneResult["Character"];
                    string name = (string)sceneResult["Name"];
                    string gameName = (string)sceneResult["GameName"];
                    string link = (string)sceneResult["Link"];

                    // Assemble tweet and subtweet
                    tweetText = $"\"{line}\"~{character}";
                    subTweetText = $"\"{name}\" from {gameName}\r\n{link}";

                    break;
                }
                sceneResult.Close();

                // Send out daily tweet
                string consumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey") ?? string.Empty;
                string consumerSecret = Environment.GetEnvironmentVariable("TwitterConsumerSecret") ?? string.Empty;
                string accessKey = Environment.GetEnvironmentVariable("TwitterAccessKey") ?? string.Empty;
                string accessSecret = Environment.GetEnvironmentVariable("TwitterAccessSecret") ?? string.Empty;

                TwitterClient userClient = new TwitterClient(consumerKey, consumerSecret, accessKey, accessSecret);


                // Tweet the results
                var tweet = userClient.Tweets.PublishTweetAsync(tweetText).Result;

                PublishTweetParameters parameters = new PublishTweetParameters
                {
                    InReplyToTweetId = tweet.Id,
                    Text = subTweetText
                };
                var subTweet = userClient.Tweets.PublishTweetAsync(parameters).Result;

                string updateQuery = $"UPDATE wayfinderprojectdb.DailyCutscenes Set HasTweeted = 1 WHERE SceneId = {dailyCutscene.SceneId}";

                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                updateCommand.ExecuteReader();
            }

            connection.Close();

            return Task.CompletedTask;
        }
    }
}