using stripIntegration.Data;
using System.Linq;
using System;

namespace stripIntegration.Seeds
{
public static class SeedExtensions
{
    public static void SeedData(AppDbContext dbContext)
    {
        // Check if there are any products already in the database
        if (!dbContext.Products.Any())
        {
            dbContext.Products.AddRange(
                new stripIntegration.Models.Product 
                { 
                    Name = "SSD 500GB", 
                    Description = "Solid State Drive for fast loading.", 
                    Price = 50.00m, 
                    Quantity = 100 
                },
                new stripIntegration.Models.Product 
                { 
                    Name = "RAM DDR5 16GB", 
                    Description = "High-speed DDR5 RAM module.", 
                    Price = 85.50m, 
                    Quantity = 80 
                },
                new stripIntegration.Models.Product 
                { 
                    Name = "Intel i5 CPU", 
                    Description = "Latest generation Core i5 processor.", 
                    Price = 220.99m, 
                    Quantity = 50 
                }
            );
            dbContext.SaveChanges();
        }
    }
}
}