using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(AccountId), Name = "Index_HistoryAccountId")]
    public class SearchHistory
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

        public string TextSearch { get; set; } = string.Empty; // Custom search, like for dialogue, line, etc.
        public string SpecificSearch { get; set; } = string.Empty; // Main row, like Scene, Interview, etc.
        public string Param1Search { get; set; } = string.Empty;
        public string Param2Search { get; set; } = string.Empty;
        public string Param3Search { get; set; } = string.Empty;
        public string Param4Search { get; set; } = string.Empty;
        public string Param5Search { get; set; } = string.Empty;
        public string Param6Search { get; set; } = string.Empty;
        public string Param7Search { get; set; } = string.Empty;
    }
}
