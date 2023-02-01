namespace WayfinderProjectAPI.Data.DTOs
{
    public class ProjectRecordDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int SpecificRecordId { get; set; }
        public string Notes { get; set; } = string.Empty;

        public ProjectDto Project { get; set; } = new();
    }
}