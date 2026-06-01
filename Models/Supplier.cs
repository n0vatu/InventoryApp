using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele furnizorului este obligatoriu!")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email-ul de contact este obligatoriu!")]
        [EmailAddress(ErrorMessage = "Formatul adresei de email nu este valid (lipsește @ sau domeniul)!")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numărul de telefon este obligatoriu!")]
      
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Numărul de telefon trebuie să conțină doar cifre (între 10 și 15 cifre, opțional cu + în față)!")]
        public string Phone { get; set; } = string.Empty;
    }
}