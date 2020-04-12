using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class BotOutputListContext : DbContext
    {
        public DbSet<BotOutputList> BotOutputList { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotOutputList>().ToTable("Bot_Output_List");
        }
    }
}