using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Data.Models
{
    [Table("JJ_Character")]
    public class JJCharacter
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string AdditionalInformation { get; set; } = string.Empty;


        [Required]
        public virtual Game Game { get; set; }
        [Required]
        public virtual Character Character { get; set; }
    }
}