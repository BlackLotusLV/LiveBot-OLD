using Microsoft.EntityFrameworkCore;

namespace LiveBot.DB
{
    public class DisciplineListContext : DbContext
    {
        public DbSet<DisciplineList> DisciplineList { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(CustomMethod.GetConnString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DisciplineList>().ToTable("Discipline_List");
        }
    }
}