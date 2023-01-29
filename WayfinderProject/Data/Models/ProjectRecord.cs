using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{
    public class ProjectRecord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; // MemoryArchive, JiminyJournal, MoogleShop
        [Required]
        public string Category { get; set; } = string.Empty; // Scenes, Reports, Interviews, etc.
        [Required]
        public int SpecificRecordId { get; set; } // Main row, like Scene, Interview, etc.
        [AllowNull]
        public string Notes { get; set; } = string.Empty;

        [AllowNull]
        public virtual Project Project { get; set; }
    }
}
