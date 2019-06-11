using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class UserImagesContext : DbContext
    {
        public DbSet<UserImages> UserImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserImages>().ToTable("User_Images");
        }
    }
}