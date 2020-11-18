using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    public class RoleTagSettingsContext : DbContext
    {
        public DbSet<RoleTagSettings> RoleTagSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleTagSettings>().ToTable("Role_Tag_Settings");
        }
    }
}