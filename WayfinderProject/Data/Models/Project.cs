using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(AccountId), Name = "Index_ProjectAccountId")]
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string AccountId { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedDate { get; set; }

        [AllowNull]
        public virtual ICollection<ProjectRecord> ProjectRecords { get; set; }
    }
}
