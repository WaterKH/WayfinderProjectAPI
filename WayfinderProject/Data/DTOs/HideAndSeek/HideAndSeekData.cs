namespace WayfinderProject.Data.DTOs.HideAndSeek
{
    public class HideAndSeekData
    {
        public string Username { get; set; }
        public string PlayerType { get; set; } // Hider, Seeker
        public string Position { get; set; }
        public string WorldName { get; set; }
        public string LevelName { get; set; } // Which sub-stage of the world
        public string RoomName { get; set; }
        public string Password { get; set; }

        // Hider Values
        public DateTime? StartHideTime { get; set; } = null;
        public string HideState { get; set; } // Not Hidden, Hiding, Hidden, Found, Stopped

        // Seeker Values
        public bool IsReady { get; set; }
        public DateTime? SeekerFoundTime { get; set; } = null;
        public int Points { get; set; }
    }
}
