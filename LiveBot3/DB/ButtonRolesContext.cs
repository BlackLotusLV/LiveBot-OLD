using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    public class ButtonRolesContext : DbContext
    {
        public DbSet<ButtonRoles> ButtonRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ButtonRoles>().ToTable("Button_Roles");
        }
    }
}