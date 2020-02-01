using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class AMBannedWordsContext : DbContext
    {
        public DbSet<AMBannedWords> AMBannedWords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AMBannedWords>().ToTable("AM_Banned_Words");
        }
    }
}