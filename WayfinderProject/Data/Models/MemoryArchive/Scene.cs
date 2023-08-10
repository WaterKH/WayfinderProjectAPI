using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using WayfinderProject.Data.Enums;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(Name), Name = "Index_SceneName")]
    [Table("MA_Scene")]
    public class Scene
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [AllowNull]
        public string Link { get; set; } = string.Empty;
        [AllowNull]
        public string Notes { get; set; } = string.Empty;
        [AllowNull]
        public State State { get; set; }


        [AllowNull]
        public virtual Game Game { get; set; }
        [AllowNull]
        public virtual Script Script { get; set; }
        [AllowNull]
        public virtual ICollection<World> Worlds { get; set; }
        [AllowNull]
        public virtual ICollection<Character> Characters { get; set; }
        [AllowNull]
        public virtual ICollection<Area> Areas { get; set; }
        [AllowNull]
        public virtual ICollection<Music> Music { get; set; }
    }
}