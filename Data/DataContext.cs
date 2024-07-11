using Microsoft.EntityFrameworkCore;
using SimpleCRUD.Model;

namespace SimpleCRUD.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<CountryCodes> CountryCodes { get; set; }

    }
}
