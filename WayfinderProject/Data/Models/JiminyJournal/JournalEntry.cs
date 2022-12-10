using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WayfinderProjectAPI.Data.Models
{
    [Table("JJ_Entry")]
    public class JournalEntry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string AdditionalInformation { get; set; } = string.Empty;


        [Required]
        public virtual Game Game { get; set; } = new();
        [Required]
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
        [Required]
        public virtual ICollection<World> Worlds { get; set; } = new List<World>();
    }
}