using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class UserSettingsContext : DbContext
    {
        public DbSet<UserSettings> UserSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSettings>().ToTable("User_Settings");
        }
    }
}