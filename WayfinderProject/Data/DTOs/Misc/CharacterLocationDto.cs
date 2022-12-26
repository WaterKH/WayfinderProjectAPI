namespace WayfinderProjectAPI.Data.DTOs
{
    public class CharacterLocationDto
    {
        public int Id { get; set; }


        public GameDto Game { get; set; } = new GameDto();
        public CharacterDto Character { get; set; } = new CharacterDto();
        public List<WorldDto> Worlds { get; set; } = new List<WorldDto>();
        public List<AreaDto> Areas { get; set; } = new List<AreaDto>();
    }
}