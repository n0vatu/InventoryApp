using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApp.Data; 
using InventoryApp.Models;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public ProductsController(InventoryDbContext context)
        {
            _context = context;
        }

        // GET: api/Products (Afișează toate produsele)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpPost]
public async Task<ActionResult<Product>> PostProduct(Product product)
{
    _context.Products.Add(product);
    await _context.SaveChangesAsync(); 

   
    if (product.StockQuantity > 0)
    {
        var initialMovement = new InventoryMovement
        {
            ProductId = product.Id,
            WarehouseId = 1, 
            Quantity = product.StockQuantity,
            Type = "Initial-Inbound",
            Date = DateTime.UtcNow
        };

        _context.InventoryMovements.Add(initialMovement);
        await _context.SaveChangesAsync();
    }

    return CreatedAtAction("GetProduct", new { id = product.Id }, product);
}
    }
}