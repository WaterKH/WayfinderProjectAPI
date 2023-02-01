namespace WayfinderProjectAPI.Data.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public List<ProjectRecordDto> ProjectRecords { get; set; } = new List<ProjectRecordDto>();
    }
}