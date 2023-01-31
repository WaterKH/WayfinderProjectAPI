namespace WayfinderProject.Data.Models
{
    public class PatreonIdentity
    {
        public Data Data { get; set; } = default!;
    }

    public class Data
    {
        public string Id { get; set; } = string.Empty;
        public Relationship Relationships { get; set; } = default!;
        public string Type { get; set; } = string.Empty;
    }

    public class Relationship
    {
        public Membership Memberships { get; set; } = default!;
    }

    public class Membership
    {
        public List<Data> Data { get; set; } = default!;
    }
}