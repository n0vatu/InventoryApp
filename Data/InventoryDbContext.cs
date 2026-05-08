using Microsoft.EntityFrameworkCore;
using InventoryApp.Models; 

namespace InventoryApp.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) 
            : base(options) { }

        
        public DbSet<Product> Products { get; set; }
    }
}