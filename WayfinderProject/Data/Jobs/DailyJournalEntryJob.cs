using MySqlConnector;
using Quartz;
using Tweetinvi;
using Tweetinvi.Parameters;
using WayfinderProject.Data.Models;

namespace WayfinderProject.Data.Jobs
{
    [DisallowConcurrentExecution]
    public class DailyEntryJob : IJob
    {
        private readonly ILogger<DailyEntryJob> _logger;

        public DailyEntryJob(ILogger<DailyEntryJob> logger)
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

            string selectQuery = $"SELECT * FROM wayfinderprojectdb.DailyJournalEntries WHERE DateCode = {dateCode}";

            MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
            MySqlDataReader selectResult = selectCommand.ExecuteReader();

            DailyJournalEntry? dailyJournalEntry = null;
            while (selectResult.Read())
            {
                dailyJournalEntry = new DailyJournalEntry
                {
                    Id = (int)selectResult["Id"],
                    DateCode = (string)selectResult["DateCode"],
                    EntryId = (int)selectResult["EntryId"],
                    HasTweeted = (bool)selectResult["HasTweeted"]
                };

                break;
            }
            selectResult.Close();

            // Only create a daily tweet if don't already have one
            if (dailyJournalEntry != null && !dailyJournalEntry.HasTweeted)
            {
                // Get random daily scene
                string journalEntryCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.JJ_Entry
                                            INNER JOIN wayfinderprojectdb.Games ON Games.Id = JJ_Entry.GameId
                                            WHERE JJ_Entry.Id = {dailyJournalEntry.EntryId}";

                MySqlCommand journalEntryCountCommand = new MySqlCommand(journalEntryCountQuery, connection);
                int journalEntryCount = Convert.ToInt32(journalEntryCountCommand.ExecuteScalar());

                string journalEntryQuery = $@"SELECT JJ_Entry.Id AS EntryId, JJ_Entry.Title AS Title, JJ_Entry.Description AS Description, JJ_Entry.Category AS Category, Games.Name AS GameName
                                    FROM wayfinderprojectdb.JJ_Entry 
                                    INNER JOIN wayfinderprojectdb.Games ON Games.Id = JJ_Entry.GameId
                                    WHERE JJ_Entry.Id = {dailyJournalEntry.EntryId}";

                MySqlCommand journalEntryCommand = new MySqlCommand(journalEntryQuery, connection);
                MySqlDataReader journalEntryResult = journalEntryCommand.ExecuteReader();

                int randomEntry = random.Next(0, journalEntryCount);
                int counter = 0;
                string tweetText = string.Empty;
                string subTweetText = string.Empty;
                while (journalEntryResult.Read())
                {
                    if (counter != randomEntry)
                    {
                        counter += 1;
                        continue;
                    }

                    string title = (string)journalEntryResult["Title"];
                    string description = (string)journalEntryResult["Description"];
                    string category = (string)journalEntryResult["Category"];
                    string gameName = (string)journalEntryResult["GameName"];

                    // Assemble tweet and subtweet
                    if (description.Length > 275)
                    {
                        var temp = description[..275];
                        temp = temp[..temp.LastIndexOf(' ')].Trim();

                        tweetText = $"{temp}...";
                    }
                    else
                    {
                        tweetText = $"{description}";
                    }
                    subTweetText = $"\"{title}\" from {gameName}\r\nRead more here: https://wayfinderprojectkh.com/jiminy_journal?entry={dailyJournalEntry.EntryId}&category={category}&open_row={dailyJournalEntry.EntryId}";

                    break;
                }
                journalEntryResult.Close();

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

                string updateQuery = $"UPDATE wayfinderprojectdb.DailyJournalEntries Set HasTweeted = 1 WHERE EntryId = {dailyJournalEntry.EntryId}";

                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                updateCommand.ExecuteReader();
            }

            connection.Close();

            return Task.CompletedTask;
        }
    }
}