namespace WayfinderProject.Data.Models.StoredProcedure
{
    public class StoredProcedureRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Parameters { get; set; } = new List<string>();
    }
}
