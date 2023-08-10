using WayfinderProject.Data.Enums;

namespace WayfinderProjectAPI.Data.DTOs
{
    public class InterviewDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string AdditionalLink { get; set; } = string.Empty;
        public State State { get; set; }


        public ICollection<InterviewLineDto> Conversation { get; set; } = new List<InterviewLineDto>();
        public virtual ICollection<GameDto> Games { get; set; } = new List<GameDto>();
        public virtual ICollection<PersonDto> Participants { get; set; } = new List<PersonDto>();
        public virtual ProviderDto Provider { get; set; } = new();
        public virtual PersonDto Translator { get; set; } = new();
    }
}