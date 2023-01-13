using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{
    [Index(nameof(Name), Name = "Index_InterviewName")]
    [Table("MA_Interview")]
    public class Interview
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Link { get; set; } = string.Empty;
        [AllowNull]
        public DateTime ReleaseDate { get; set; }
        [AllowNull]
        public string AdditionalLink { get; set; } // Usually used for Videos


        [AllowNull]
        public virtual ICollection<Participant> Participants { get; set; }
        [AllowNull]
        public virtual ICollection<InterviewLine> Conversation { get; set; }
        [AllowNull]
        public virtual ICollection<Game> Games { get; set; }

        [AllowNull]
        public virtual Provider Provider { get; set; }
        [AllowNull]
        public virtual Translator Translator { get; set; }
    }
}