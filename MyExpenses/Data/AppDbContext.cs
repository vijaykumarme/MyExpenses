using Microsoft.EntityFrameworkCore;
using MyExpenses.Models;

namespace MyExpenses.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Expenses> expenses { get; set; }
        public DbSet<Category> categories { get; set; }
    }
}
