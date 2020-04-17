using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class WeatherScheduleContext : DbContext
    {
        public DbSet<WeatherSchedule> WeatherSchedule { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherSchedule>().ToTable("Weather_Schedule");
        }
    }
}