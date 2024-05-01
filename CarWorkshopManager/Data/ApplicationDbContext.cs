using CarWorkshopManager.Models;
using Microsoft.EntityFrameworkCore;
namespace CarWorkshopManager.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Part> Parts { get; set; }

        public DbSet<UserAuth> userAuths { get; set; }

        // Add other DbSets here
    }
}