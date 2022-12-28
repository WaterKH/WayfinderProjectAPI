namespace WayfinderProjectAPI.Data.DTOs
{
    public class RecipeMaterialDto
    {
        public int Id { get; set; }
        public int Amount { get; set; }

        public virtual InventoryDto Inventory { get; set; } = new();
    }
}