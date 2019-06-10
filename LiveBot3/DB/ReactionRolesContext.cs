using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    public class ReactionRolesContext : DbContext
    {

        public DbSet<ReactionRoles> ReactionRoles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReactionRoles>().ToTable("Reaction_Roles");
        }
    }
}
