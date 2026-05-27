namespace InventoryApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int SupplierId { get; set; } 

        public int Pret {get; set; }
    }
}