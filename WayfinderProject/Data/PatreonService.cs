using System.Net.Http.Headers;
using WayfinderProject.Data.Models;
using WayfinderProjectAPI.Data;

namespace WayfinderProject.Data
{
    public class PatreonService
    {
        private HttpClient HttpClient { get; set; } = default!;
        public PatreonService(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
        }

        public async Task GetIdentity(WayfinderContext context, string accountId, string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.patreon.com/api/oauth2/token");
            var requestContent = new MultipartFormDataContent
            {
                { new StringContent(code), "code" },
                { new StringContent("authorization_code"), "grant_type" },
                { new StringContent(Environment.GetEnvironmentVariable("PatreonClientId") ?? ""), "client_id" },
                { new StringContent(Environment.GetEnvironmentVariable("PatreonClientSecret") ?? ""), "client_secret" },
                { new StringContent("https://wayfinderprojectkh.com/"), "redirect_uri" }
            };

            request.Content = requestContent;

            using var httpResponse = await HttpClient.SendAsync(request);

            var response = await httpResponse.Content.ReadFromJsonAsync<PatreonTokenResponse>();

            if (!httpResponse.IsSuccessStatusCode || response == null)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new BadHttpRequestException(httpResponse?.ReasonPhrase);
                }

                throw new Exception();
            }

            if (response != null)
            {
                // Save access and refresh token to user
                var user = context.Users.First(x => x.Id == accountId);

                user.PatreonAccessToken = response.access_token;
                user.PatreonRefreshToken = response.refresh_token;

                context.SaveChanges();
            }
        }

        public async Task<bool> IsPatron(WayfinderContext context, string accountId)
        {
            var user = context.Users.First(x => x.Id == accountId);

            return await this.IsPatron(user);
        }

        public async Task<bool> IsPatron(WayfinderProjectUser user)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.patreon.com/api/oauth2/v2/identity");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.PatreonAccessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var httpResponse = await HttpClient.SendAsync(request);

            var response = await httpResponse.Content.ReadFromJsonAsync<PatreonIdentity>();

            if (!httpResponse.IsSuccessStatusCode || response == null)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }

                throw new Exception();
            }

            return response.Data.Relationships?.Memberships.Data.Any() ?? false;
        }

        public async Task RefreshToken(WayfinderContext context, string accountId)
        {
            var user = context.Users.First(x => x.Id == accountId);

            var request = new HttpRequestMessage(HttpMethod.Post, "www.patreon.com/api/oauth2/token?include=memberships");
            var requestContent = new MultipartFormDataContent
            {
                { new StringContent("refresh_token"), "grant_type" },
                { new StringContent(user.PatreonRefreshToken), "refresh_token" },
                { new StringContent(Environment.GetEnvironmentVariable("PatreonClientId") ?? ""), "client_id" },
                { new StringContent(Environment.GetEnvironmentVariable("PatreonClientSecret") ?? ""), "client_secret" }
            };

            request.Content = requestContent;

            using var httpResponse = await HttpClient.SendAsync(request);

            var response = await httpResponse.Content.ReadFromJsonAsync<PatreonTokenResponse>();

            if (response != null)
            {
                // Save access and refresh token to user
                user.PatreonAccessToken = response.access_token;
                user.PatreonRefreshToken = response.refresh_token;

                context.SaveChanges();
            }
        }
    }
}