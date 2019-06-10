using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    class ServerRanksContext : DbContext
    {
        public DbSet<ServerRanks> ServerRanks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServerRanks>().ToTable("Server_Ranks");
        }
    }
}
