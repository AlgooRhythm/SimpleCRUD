using Microsoft.EntityFrameworkCore;
using SimpleCRUD.Model;
using System.Diagnostics.Metrics;

namespace SimpleCRUD.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<CountryCodes> CountryCodes { get; set; }
        public DbSet<ClassificationCodes> ClassificationCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CountryCodes>()
                .HasKey(c => c.Code); // Set Code as the primary key

            modelBuilder.Entity<ClassificationCodes>()
                .HasKey(c => c.Code); // Set Code as the primary key
        }
    }
}
