namespace WayfinderProjectAPI.Data.DTOs
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string UnlockConditionDescription { get; set; } = string.Empty;


        public virtual GameDto Game { get; set; } = new();
        public virtual List<InventoryDto> MaterialsNeeded { get; set; } = new();
        public virtual List<int> AmountPerMaterial { get; set; } = new();
    }
}