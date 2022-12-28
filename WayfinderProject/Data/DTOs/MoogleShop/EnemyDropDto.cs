namespace WayfinderProjectAPI.Data.DTOs
{
    public class EnemyDropDto
    {
        public int Id { get; set; }
        public float DropRate { get; set; } = 0.0f;
        public string AdditionalInformation { get; set; } = string.Empty;


        public virtual CharacterLocationDto CharacterLocation { get; set; } = new();
        public virtual InventoryDto Inventory { get; set; } = new();
    }
}