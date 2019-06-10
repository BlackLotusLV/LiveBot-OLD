using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    class UserWarningsContext : DbContext
    {
        public DbSet<UserWarnings> UserWarnings { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserWarnings>().ToTable("User_Warnings");
        }
    }
}
      
