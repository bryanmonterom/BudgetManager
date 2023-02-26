using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The field {0} is required")]
        [StringLength(maximumLength:50,ErrorMessage ="This field cannot be longer that {1} characters")]
        public string Name { get; set; }
        [Display(Name ="Operation Type")]
        public OperationType IdTransactionType { get; set; }
        public int IdUser { get; set; }


    }
}
