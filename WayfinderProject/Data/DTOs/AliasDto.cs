namespace WayfinderProjectAPI.Data.DTOs
{
    public class AliasDto
    {
        public int Id { get; set; }
        public string Original { get; set; } = string.Empty;
        public string AppearAs { get; set; } = string.Empty;
    }
}