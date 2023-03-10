using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field must be a valid email address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
