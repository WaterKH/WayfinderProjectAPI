namespace WayfinderProjectAPI.Data.DTOs
{
    public class JJCharacterDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public string AdditionalInformation { get; set; } = string.Empty;


        public GameDto Game { get; set; }
        public CharacterDto Character { get; set; }
    }
}