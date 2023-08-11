using WayfinderProject.Data.Enums;

namespace WayfinderProjectAPI.Data.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AdditionalInformation { get; set; } = string.Empty;
        public int Cost { get; set; } = 0;
        public string Currency { get; set; } = string.Empty;
        public State State { get; set; }

        public virtual GameDto Game { get; set; } = new();
        public virtual List<EnemyDropDto> EnemyDrops { get; set; } = new();
    }
}