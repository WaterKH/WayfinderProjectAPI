namespace WayfinderProjectAPI.Data.DTOs
{
    public class FavoriteDto
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int SpecificRecordId { get; set; }
    }
}