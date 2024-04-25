using CarWorkshopManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CarWorkshopManager.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any employees already in the database.
                if (context.Employees.Any())
                {
                    return;   // DB has been seeded
                }

                context.Employees.AddRange(
                    new Employee
                    {
                        Name = "Alice",
                        HourlyRate = 30.00M
                    },

                    new Employee
                    {
                        Name = "Bob",
                        HourlyRate = 35.00M
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
