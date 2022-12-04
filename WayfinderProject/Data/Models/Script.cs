using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(GameName), nameof(SceneName), Name = "Index_GameSceneName")]
    public class Script
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string GameName { get; set; } = string.Empty;
        [Required]
        public string SceneName { get; set; } = string.Empty;


        [AllowNull]
        public virtual ICollection<Scene> Scenes { get; set; }
        [AllowNull]
        public virtual ICollection<ScriptLine> Lines { get; set; }
    }
}