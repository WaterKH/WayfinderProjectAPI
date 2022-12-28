using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WayfinderProjectAPI.Data.Models
{
    [Table("MS_EnemyDrop")]
    public class EnemyDrop
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public float DropRate { get; set; } = 0.0f;
        [Required]
        public string AdditionalInformation { get; set; } = string.Empty;


        [Required]
        public virtual CharacterLocation CharacterLocation { get; set; } = new();
        [Required]
        public virtual Inventory Inventory { get; set; } = new();
    }
}