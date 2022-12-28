using Microsoft.EntityFrameworkCore;

namespace WayfinderProject.Data.Models
{
    [Index(nameof(DateCode), Name = "Index_DateCode")]
    public class DailyCutscene
    {
        public int Id { get; set; }
        public string DateCode { get; set; } = string.Empty;
        public int SceneId { get; set; }
        public bool HasTweeted { get; set; }
    }
}
