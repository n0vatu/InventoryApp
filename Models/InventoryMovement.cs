namespace InventoryApp.Models
{
    public class InventoryMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; } 
        public int Quantity { get; set; }    
        public string Type { get; set; } = "In"; 
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}