using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(AccountId), Name = "Index_FavoriteAccountId")]
    public class Favorite
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string AccountId { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; // MemoryArchive, JiminyJournal, MoogleShop
        [Required]
        public string Category { get; set; } = string.Empty; // Scenes, Reports, Interviews, etc.
        [Required]
        public int SpecificRecordId { get; set; } // Main row, like Scene, Interview, etc.
    }
}
