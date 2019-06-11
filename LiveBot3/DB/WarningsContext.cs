using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class WarningsContext : DbContext
    {
        public DbSet<Warnings> Warnings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Warnings>().ToTable("Warnings");
        }
    }
}