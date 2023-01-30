namespace WayfinderProjectAPI.Data.DTOs
{
    public class SearchSettingsDto
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;

        public bool AutoSearch { get; set; } = true;
        public bool AutoExpandFirstResult { get; set; } = false;
        public bool MainSearchEverything { get; set; } = false;
        public bool TrackHistory { get; set; } = true;
        public bool FavouriteSearch { get; set; } = false;
        public bool ProjectSearch { get; set; } = false;
    }
}