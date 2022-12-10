namespace WayfinderProjectAPI.Data.DTOs
{
    public class ScriptDto
    {
        public int Id { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string SceneName { get; set; } = string.Empty;

        public ICollection<ScriptLineDto> Lines { get; set; } = new List<ScriptLineDto>();
    }
}