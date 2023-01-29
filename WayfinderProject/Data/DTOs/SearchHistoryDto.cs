namespace WayfinderProjectAPI.Data.DTOs
{
    public class SearchHistoryDto
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string TextSearch { get; set; } = string.Empty;
        public string SpecificSearch { get; set; } = string.Empty;
        public string Param1Search { get; set; } = string.Empty;
        public string Param2Search { get; set; } = string.Empty;
        public string Param3Search { get; set; } = string.Empty;
        public string Param4Search { get; set; } = string.Empty;
        public string Param5Search { get; set; } = string.Empty;
        public string Param6Search { get; set; } = string.Empty;
        public string Param7Search { get; set; } = string.Empty;
    }
}