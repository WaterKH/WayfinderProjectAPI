namespace WayfinderProjectAPI.Data.DTOs
{
    public class ScriptLineDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Character { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
    }
}