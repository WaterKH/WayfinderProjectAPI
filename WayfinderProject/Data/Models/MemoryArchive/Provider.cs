using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(Name), Name = "Index_ProviderName")]
    public class Provider
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [AllowNull]
        public string Description { get; set; } = string.Empty;
        [AllowNull]
        public string Link { get; set; } = string.Empty;

        [AllowNull]
        public virtual ICollection<Interview> Interviews { get; set; }
    }
}