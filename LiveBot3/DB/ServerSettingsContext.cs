using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class ServerSettingsContext : DbContext
    {
        public DbSet<ServerSettings> ServerSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServerSettings>().ToTable("Server_Settings");
        }
    }
}