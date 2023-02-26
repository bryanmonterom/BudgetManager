using BudgetManager.Validations;
using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The field {0} is required")]
        [StringLength(50)]
        [FirstCapsLetter]
        public string Name { get; set; }
        [Display(Name="Account Type")]
        public int IdAccountType { get; set; }
        public decimal Balance { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public string AccountType { get; set; }
       
    }

   
}
