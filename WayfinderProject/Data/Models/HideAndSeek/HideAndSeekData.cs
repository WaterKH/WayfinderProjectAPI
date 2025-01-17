using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProject.Data.Models.HideAndSeek
{
    public class HideAndSeekData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [AllowNull]
        public string? PlayerType { get; set; } // Hider, Seeker
        [AllowNull]
        public string? Character { get; set; }
        [AllowNull]
        public string? Position { get; set; }
        [AllowNull]
        public string? WorldName { get; set; }
        [AllowNull]
        public string? LevelName { get; set; } // Which sub-stage of the world
        [AllowNull]
        public string? RoomName { get; set; }
        [AllowNull]
        public string? Password { get; set; }

        // Hider Values
        [AllowNull]
        public DateTime? StartHideTime { get; set; }
        [Required]
        public string? HideState { get; set; } // Not Hidden, Hiding, Hidden, Found, Stopped

        // Seeker Values
        [Required]
        public bool? IsReady { get; set; }
        [AllowNull]
        public DateTime? SeekerFoundTime { get; set; }
        [Required]
        public int? Points { get; set; }
    }
}
