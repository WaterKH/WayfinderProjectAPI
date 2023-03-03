using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(AccountId), Name = "Index_SettingsAccountId")]
    public class SearchSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string AccountId { get; set; } = string.Empty;

        [Required]
        public bool AutoSearch { get; set; } = true;
        [Required]
        public bool AutoExpandFirstResult { get; set; } = false;
        [Required]
        public bool MainSearchEverything { get; set; } = false;
        [Required]
        public bool TrackHistory { get; set; } = true;
        [Required]
        public bool FavouriteSearch { get; set; } = false;
        [Required]
        public bool ProjectSearch { get; set; } = false;
        [Required]
        public bool IncludeAlias { get; set; } = false;
        [Required]
        public string ResultsDisplay { get; set; } = "Table";
    }
}
