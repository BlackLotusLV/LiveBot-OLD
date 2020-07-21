using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    internal class ModMailContext : DbContext
    {
        public DbSet<ModMail> ModMail { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModMail>().ToTable("Mod_Mail");
        }
    }
}