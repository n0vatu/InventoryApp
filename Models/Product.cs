using System.ComponentModel.DataAnnotations; 

namespace InventoryApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele produsului este obligatoriu!")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prețul este obligatoriu!")]
        public int Pret { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Trebuie să selectați un furnizor!")]
        public int SupplierId { get; set; }
    }
}