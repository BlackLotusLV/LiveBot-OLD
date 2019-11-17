using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class CommandsUsedCountContext : DbContext
    {
        public DbSet<CommandsUsedCount> CommandsUsedCount { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommandsUsedCount>().ToTable("Commands_Used_Count");
        }
    }
}