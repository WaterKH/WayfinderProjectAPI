using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WayfinderProject.Data.Models;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Data
{
    public class WayfinderContext : IdentityDbContext<WayfinderProjectUser>
    {
        public WayfinderContext(DbContextOptions<WayfinderContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Area>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Character>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Game>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Music>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Scene>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<World>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Script>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Interview>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<InterviewLine>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Participant>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Provider>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Translator>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<JournalEntry>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Recipe>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Inventory>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<EnemyDrop>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<CharacterLocation>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<DailyCutscene>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<DailyJournalEntry>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<DailyMoogleRecord>().Property(p => p.Id).ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Character> Characters { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Music> Music { get; set; } = null!;
        public DbSet<Scene> Scenes { get; set; } = null!;
        public DbSet<World> Worlds { get; set; } = null!;
        public DbSet<Script> Script { get; set; } = null!;

        public DbSet<Interview> Interviews { get; set; } = null!;
        public DbSet<InterviewLine> InterviewLines { get; set; } = null!;
        public DbSet<Participant> Participants { get; set; } = null!;
        public DbSet<Provider> Providers { get; set; } = null!;
        public DbSet<Translator> Translators { get; set; } = null!;

        public DbSet<JournalEntry> JournalEntries { get; set; } = null!;

        public DbSet<Recipe> Recipes { get; set; } = null!;
        public DbSet<Inventory> Inventory { get; set; } = null!;
        public DbSet<EnemyDrop> EnemyDrops { get; set; } = null!;

        public DbSet<CharacterLocation> CharacterLocations { get; set; } = null!;

        public DbSet<DailyCutscene> DailyCutscenes { get; set; } = null!;
        public DbSet<DailyJournalEntry> DailyJournalEntries { get; set; } = null!;
        public DbSet<DailyMoogleRecord> DailyMoogleRecords { get; set; } = null!;
    }
}