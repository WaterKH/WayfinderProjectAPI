using WayfinderProject.Data.Enums;

namespace WayfinderProjectAPI.Data.DTOs
{
    public class InteractionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public State State { get; set; }


        public GameDto Game { get; set; } = new GameDto();
        public ScriptDto Script { get; set; } = new ScriptDto();
        public ICollection<WorldDto> Worlds { get; set; } = new List<WorldDto>();
        public ICollection<CharacterDto> Characters { get; set; } = new List<CharacterDto>();
        public ICollection<AreaDto> Areas { get; set; } = new List<AreaDto>();
        public ICollection<MusicDto> Music { get; set; } = new List<MusicDto>();
    }
}