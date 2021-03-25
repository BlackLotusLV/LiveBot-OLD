using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class ServerWelcomeSettingsContext : DbContext
    {
        public DbSet<ServerWelcomeSettings> ServerWelcomeSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServerWelcomeSettings>().ToTable("Server_Welcome_Settings");
        }
    }
}