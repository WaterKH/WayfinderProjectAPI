namespace WayfinderProjectAPI.Data.DTOs
{
    public class InterviewLineDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Speaker { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
    }
}