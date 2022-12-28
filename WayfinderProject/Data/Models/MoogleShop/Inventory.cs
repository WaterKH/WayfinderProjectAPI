using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WayfinderProjectAPI.Data.Models
{
    [Table("MS_Inventory")]
    public class Inventory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string AdditionalInformation { get; set; } = string.Empty;
        [Required]
        public int Cost { get; set; } = 0;
        [Required]
        public string Currency { get; set; } = string.Empty;


        [Required]
        public virtual Game Game { get; set; } = new();
        [Required]
        public virtual List<EnemyDrop> EnemyDrops { get; set; } = new();
    }
}