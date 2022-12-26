using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(Name), Name = "Index_CharacterName")]
    public class Character
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;


        [AllowNull]
        public virtual ICollection<CharacterLocation> CharacterLocations { get; set; }

        [AllowNull]
        public virtual ICollection<Scene> Scenes { get; set; }
        [AllowNull]
        public virtual ICollection<JournalEntry> JournalEntries { get; set; }
    }
}