using MySqlConnector;
using Quartz;
using Tweetinvi;
using Tweetinvi.Parameters;
using WayfinderProject.Data.Models;

namespace WayfinderProject.Data.Jobs
{
    [DisallowConcurrentExecution]
    public class DailyMoogleRecordJob : IJob
    {
        private readonly ILogger<DailyMoogleRecordJob> _logger;

        public DailyMoogleRecordJob(ILogger<DailyMoogleRecordJob> logger)
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

            string selectQuery = $"SELECT * FROM wayfinderprojectdb.DailyMoogleRecords WHERE DateCode = {dateCode}";

            MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
            MySqlDataReader selectResult = selectCommand.ExecuteReader();

            DailyMoogleRecord? dailyMoogleRecord = null;
            while (selectResult.Read())
            {
                dailyMoogleRecord = new DailyMoogleRecord
                {
                    Id = (int)selectResult["Id"],
                    DateCode = (string)selectResult["DateCode"],
                    RecordId = (int)selectResult["RecordId"],
                    HasTweeted = (bool)selectResult["HasTweeted"]
                };

                break;
            }
            selectResult.Close();

            // Only create a daily tweet if don't already have one
            if (dailyMoogleRecord != null && !dailyMoogleRecord.HasTweeted)
            {
                // Get random daily scene
                string moogleRecordCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.MS_Inventory 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = MS_Inventory.GameId
                                    WHERE MS_Inventory.Id = {dailyMoogleRecord.RecordId}";

                MySqlCommand moogleRecordCountCommand = new MySqlCommand(moogleRecordCountQuery, connection);
                int moogleRecordCount = Convert.ToInt32(moogleRecordCountCommand.ExecuteScalar());

                string moogleRecordQuery = $@"SELECT MS_Inventory.Id AS RecordId, MS_Inventory.Name AS Name, MS_Inventory.Description AS Description, MS_Inventory.Category AS Category, Games.Name AS GameName
                                    FROM wayfinderprojectdb.MS_Inventory 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = MS_Inventory.GameId
                                    WHERE MS_Inventory.Id = {dailyMoogleRecord.RecordId}";

                MySqlCommand moogleRecordCommand = new MySqlCommand(moogleRecordQuery, connection);
                MySqlDataReader moogleRecordResult = moogleRecordCommand.ExecuteReader();

                int randomEntry = random.Next(0, moogleRecordCount);
                int counter = 0;
                string tweetText = string.Empty;
                string subTweetText = string.Empty;
                while (moogleRecordResult.Read())
                {
                    if (counter != randomEntry)
                    {
                        counter += 1;
                        continue;
                    }

                    string name = (string)moogleRecordResult["Name"];
                    string description = (string)moogleRecordResult["Description"];
                    string category = (string)moogleRecordResult["Category"];
                    string gameName = (string)moogleRecordResult["GameName"];

                    // Assemble tweet and subtweet
                    if (description.Length > 275)
                    {
                        var temp = description[..275];
                        temp = temp[..temp.LastIndexOf(' ')].Trim();

                        tweetText = $"{temp}...";
                    }
                    else
                    {
                        tweetText = $"{name}, kupo!\r\n\r\n{description}";
                    }
                    subTweetText = $"\"{name}\" from {gameName}\r\nRead more here: https://wayfinderprojectkh.com/moogle_shop?record={dailyMoogleRecord.RecordId}&category={category}&open_row={dailyMoogleRecord.RecordId}";

                    break;
                }
                moogleRecordResult.Close();

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

                string updateQuery = $"UPDATE wayfinderprojectdb.DailyMoogleRecords Set HasTweeted = 1 WHERE SceneId = {dailyMoogleRecord.RecordId}";

                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                updateCommand.ExecuteReader();
            }

            connection.Close();

            return Task.CompletedTask;
        }
    }
}