namespace WayfinderProjectAPI.Data.DTOs
{
    public class SceneDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;


        public GameDto Game { get; set; }
        public ScriptDto Script { get; set; }
        public ICollection<WorldDto> Worlds { get; set; }
        public ICollection<CharacterDto> Characters { get; set; }
        public ICollection<AreaDto> Areas { get; set; }
        public ICollection<MusicDto> Music { get; set; }
    }
}