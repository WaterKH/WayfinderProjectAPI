using Microsoft.EntityFrameworkCore;

namespace WayfinderProject.Data.Models
{
    [Index(nameof(DateCode), Name = "Index_DateCode")]
    public class DailyMoogleRecord
    {
        public int Id { get; set; }
        public string DateCode { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public bool HasTweeted { get; set; }
    }
}
