using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele halei este obligatoriu!")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Locația halei este obligatorie!")]
        public string Location { get; set; } = string.Empty;
    }
}