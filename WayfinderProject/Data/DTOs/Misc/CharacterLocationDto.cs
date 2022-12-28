namespace WayfinderProjectAPI.Data.DTOs
{
    public class CharacterLocationDto
    {
        public int Id { get; set; }


        public GameDto Game { get; set; } = new GameDto();
        public CharacterDto Character { get; set; } = new CharacterDto();
        public WorldDto World { get; set; } = new WorldDto();
        public List<AreaDto> Areas { get; set; } = new List<AreaDto>();
    }
}