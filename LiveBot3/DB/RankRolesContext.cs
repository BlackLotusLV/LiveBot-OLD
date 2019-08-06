using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class RankRolesContext : DbContext
    {
        public DbSet<RankRoles> RankRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RankRoles>().ToTable("Rank_Roles");
        }
    }
}