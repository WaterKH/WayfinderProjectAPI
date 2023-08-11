using WayfinderProject.Data.Enums;

namespace WayfinderProjectAPI.Data.DTOs
{
    public class JournalEntryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AdditionalInformation { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public State State { get; set; }


        public GameDto Game { get; set; } = new GameDto();
        public List<CharacterDto> Characters { get; set; } = new List<CharacterDto>();
        public List<WorldDto> Worlds { get; set; } = new List<WorldDto>();
    }
}