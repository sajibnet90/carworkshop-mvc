using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CarWorkshopManager.Data
{
    public class ApplicationDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
