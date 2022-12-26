using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WayfinderProjectAPI.Data.Models
{
    public class CharacterLocation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual Character Character { get; set; } = new();
        [Required]
        public virtual Game Game { get; set; } = new();

        [AllowNull]
        public virtual ICollection<World> Worlds { get; set; }
        [AllowNull]
        public virtual ICollection<Area> Areas { get; set; }
    }
}