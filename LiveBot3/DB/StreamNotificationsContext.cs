using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    public class StreamNotificationsContext : DbContext
    {
        public DbSet<StreamNotifications> StreamNotifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StreamNotifications>().ToTable("Stream_Notification");
        }
    }
}