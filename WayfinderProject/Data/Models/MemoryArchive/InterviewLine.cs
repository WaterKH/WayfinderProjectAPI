using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{
    public class InterviewLine
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public int Order { get; set; }
        [Required]
        public string Speaker { get; set; } = string.Empty;
        [Required]
        public string Line { get; set; } = string.Empty;


        [AllowNull]
        public virtual Interview Interview { get; set; }
    }
}