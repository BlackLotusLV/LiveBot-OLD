using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    class LeaderboardContext : DbContext
    {
        public DbSet<Leaderboard> Leaderboard { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Leaderboard>().ToTable("Leaderboard");
        }
    }
}
