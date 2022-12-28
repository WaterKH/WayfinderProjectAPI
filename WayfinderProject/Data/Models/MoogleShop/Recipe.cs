using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WayfinderProjectAPI.Data.Models
{
    [Table("MS_Recipe")]
    public class Recipe
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string UnlockConditionDescription { get; set; } = string.Empty;
        // TODO Create User Entry for if this is crafted or not


        [Required]
        public virtual Game Game { get; set; } = new();
        [Required]
        public virtual List<RecipeMaterial> RecipeMaterials { get; set; } = new();
    }
}