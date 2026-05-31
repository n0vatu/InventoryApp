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
// PUT: api/InventoryMovements/{id} -> Actualizează complet o mișcare
[HttpPut("{id}")]
public async Task<IActionResult> PutMovement(int id, InventoryMovement movement)
{
    if (id != movement.Id)
    {
        return BadRequest("ID-urile nu se potrivesc.");
    }

    _context.Entry(movement).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.InventoryMovements.Any(e => e.Id == id)) return NotFound();
        throw;
    }

    return NoContent();
}

// PATCH: api/InventoryMovements/{id} -> Modifică doar cantitatea sau tipul
[HttpPatch("{id}")]
public async Task<IActionResult> PatchMovement(int id, [FromBody] Dictionary<string, object> updates)
{
    var movement = await _context.InventoryMovements.FindAsync(id);
    if (movement == null) return NotFound();

    if (updates.ContainsKey("quantity")) movement.Quantity = Convert.ToInt32(updates["quantity"]);
    if (updates.ContainsKey("type")) movement.Type = updates["type"]?.ToString() ?? movement.Type;
    if (updates.ContainsKey("warehouseId")) movement.WarehouseId = Convert.ToInt32(updates["warehouseId"]);
    if (updates.ContainsKey("productId")) movement.ProductId = Convert.ToInt32(updates["productId"]);

    await _context.SaveChangesAsync();
    return NoContent();
}

// DELETE: api/InventoryMovements/{id} -> Șterge o mișcare de stoc
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteMovement(int id)
{
    var movement = await _context.InventoryMovements.FindAsync(id);
    if (movement == null)
    {
        return NotFound();
    }

    _context.InventoryMovements.Remove(movement);
    await _context.SaveChangesAsync();

    return NoContent();
}
    }
}