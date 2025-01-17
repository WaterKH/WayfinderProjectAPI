namespace WayfinderProject.Data.DTOs.HideAndSeek
{
    public class ConnectionResponse
    {
        public List<HideAndSeekData> HideAndSeekData { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
