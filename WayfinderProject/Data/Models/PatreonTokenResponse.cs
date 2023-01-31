namespace WayfinderProject.Data.Models
{
    public class PatreonTokenResponse
    {
        public string access_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public long expires_in { get; set; }
        public string scope { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
    }
}