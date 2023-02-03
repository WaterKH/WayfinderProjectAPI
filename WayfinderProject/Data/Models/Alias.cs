using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(Original), Name = "Index_AliasOriginal")]
    public class Alias
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Original { get; set; } = string.Empty;
        [Required]
        public string AppearAs { get; set; } = string.Empty;
    }
}