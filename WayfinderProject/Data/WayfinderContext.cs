using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WayfinderProjectAPI.Data.Models;

namespace WayfinderProjectAPI.Data
{
    public class WayfinderContext : DbContext
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
        }

        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Character> Characters { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Music> Music { get; set; } = null!;
        public DbSet<Scene> Scenes { get; set; } = null!;
        public DbSet<World> Worlds { get; set; } = null!;
        public DbSet<Script> Script { get; set; } = null!;
    }
}