using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApp.Data;
using InventoryApp.Models;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementsController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public MovementsController(InventoryDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryMovement>>> GetMovements()
            {
                return await _context.InventoryMovements.OrderByDescending(m => m.Date).ToListAsync();
            }
       
       [HttpPost("transfer")]
public async Task<IActionResult> TransferStock(int productId, int fromWarehouseId, int toWarehouseId, int quantity)
{
   
    var product = await _context.Products.FindAsync(productId);
    if (product == null)
    {
        return NotFound("Produsul nu a fost găsit.");
    }
    
    var outMovement = new InventoryMovement {
        ProductId = productId,
        WarehouseId = fromWarehouseId,
        Quantity = -quantity,
        Type = "Transfer-Out",
        Date = DateTime.UtcNow
    };

    var inMovement = new InventoryMovement {
        ProductId = productId,
        WarehouseId = toWarehouseId,
        Quantity = quantity,
        Type = "Transfer-In",
        Date = DateTime.UtcNow
    };

    _context.InventoryMovements.AddRange(outMovement, inMovement);
    await _context.SaveChangesAsync();

    return Ok("Transfer realizat cu succes între hale!");
}
    }
}