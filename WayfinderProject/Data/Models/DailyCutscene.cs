using Microsoft.EntityFrameworkCore;

namespace WayfinderProject.Data.Models
{
    [Index(nameof(DateCode), Name = "Index_DateCode")]
    public class DailyCutscene
    {
        public int Id { get; set; }
        public string DateCode { get; set; }
        public int SceneId { get; set; }
    }
}
