using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApp.Data; 
using InventoryApp.Models;
using System.Text.Json;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
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

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

     
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

       
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id, [FromBody] JsonElement updates)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (updates.TryGetProperty("name", out var nameProp))
            {
                product.Name = nameProp.GetString() ?? string.Empty;
            }

            if (updates.TryGetProperty("stockQuantity", out var stockProp))
            {
                product.StockQuantity = stockProp.GetInt32();
            }

            if (updates.TryGetProperty("supplierId", out var supplierProp))
            {
                product.SupplierId = supplierProp.GetInt32();
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}