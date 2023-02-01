// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using WayfinderProject.Data;
using WayfinderProject.Data.Models;

namespace WayfinderProject.Areas.Identity.Pages.Account.Manage
{
    public class QuotasModel : PageModel
    {
        private readonly UserManager<WayfinderProjectUser> _userManager;
        private readonly SignInManager<WayfinderProjectUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public QuotasModel(
            UserManager<WayfinderProjectUser> userManager,
            SignInManager<WayfinderProjectUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        public bool IsPatron { get; set; }

        public string FavouriteCount { get; set; } = "0";
        public string ProjectCount { get; set; } = "0";
        public Dictionary<string, string> ProjectRecordCount { get; set; } = new Dictionary<string, string>();
        public string APICount { get; set; } = "0";

        public string FavouriteLimit { get; set; } = "0";
        public string ProjectLimit { get; set; } = "0";
        public string ProjectRecordLimit { get; set; } = "0";
        public string APILimit { get; set; } = "0";

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Load quotas

            // Check Patron Status
            await this.GetPatronStatus(user);

            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            // Get Counts
            this.GetFavouriteCount(user, connection);
            this.GetProjectCount(user, connection);
            this.GetProjectRecordCount(user, connection);

            // Get Limits
            this.FavouriteLimit = this.IsPatron ? "650" : "200";
            this.ProjectLimit = this.IsPatron ? "20" : "5";
            this.ProjectRecordLimit = this.IsPatron ? "500" : "100";
            this.APILimit = this.IsPatron ? "No Limit" : "5000";

            if (int.TryParse(this.APILimit, out int limit))
            {
                this.APICount = (limit - user.ApiCallQuota).ToString();
            }
            else
            {
                this.APICount = "No Limit";
            }

            connection.Close();

            return Page();
        }

        public async Task GetPatronStatus(WayfinderProjectUser user)
        {
            var patreonService = new PatreonService(new HttpClient());

            if (!string.IsNullOrEmpty(user.PatreonAccessToken) && !string.IsNullOrEmpty(user.PatreonRefreshToken))
            {
                this.IsPatron = await patreonService.IsPatron(user);
            }
            else
            {
                this.IsPatron = false;
            }
        }


        public void GetFavouriteCount(WayfinderProjectUser user, MySqlConnection connection)
        {
            try
            {
                string favouriteCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.Favorites WHERE AccountId = '{user.Id}'";

                MySqlCommand favouriteCountCommand = new MySqlCommand(favouriteCountQuery, connection);
                this.FavouriteCount = ((long)favouriteCountCommand.ExecuteScalar()).ToString();
            }
            catch (Exception)
            {
                this.FavouriteCount = "0";
            }
        }

        public void GetProjectCount(WayfinderProjectUser user, MySqlConnection connection)
        {
            try
            {
                string projectCountQuery = $@"SELECT COUNT(*) AS RowCount FROM wayfinderprojectdb.Projects WHERE AccountId = '{user.Id}'";

                MySqlCommand projectCountCommand = new MySqlCommand(projectCountQuery, connection);
                this.ProjectCount = ((long)projectCountCommand.ExecuteScalar()).ToString();
            }
            catch (Exception)
            {
                this.ProjectCount = "0";
            }
        }

        public void GetProjectRecordCount(WayfinderProjectUser user, MySqlConnection connection)
        {
            try
            {
                string projectRecordsQuery = $@"SELECT p.Name AS ProjectName, COUNT(pr.Id) AS ProjectRecordCount FROM wayfinderprojectdb.ProjectRecords pr
	                                                    RIGHT JOIN wayfinderprojectdb.Projects p ON pr.ProjectId = p.Id
                                                        WHERE p.AccountId = '{user.Id}'
                                                        GROUP BY p.Name;";

                MySqlCommand projectRecordCommand = new MySqlCommand(projectRecordsQuery, connection);
                MySqlDataReader projectRecordResult = projectRecordCommand.ExecuteReader();

                while (projectRecordResult.Read())
                {
                    this.ProjectRecordCount.Add((string)projectRecordResult["ProjectName"], ((long)projectRecordResult["ProjectRecordCount"]).ToString());
                }
            }
            catch (Exception)
            {
                this.ProjectRecordCount = new Dictionary<string, string> { { "Empty", "0" } };
            }
        }
    }
}
