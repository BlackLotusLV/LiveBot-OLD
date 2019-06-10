using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    class BackgroundImageContext : DbContext
    {
        public DbSet<BackgroundImage> BackgroundImage { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BackgroundImage>().ToTable("Background_Image");
        }
    }
}
