using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        [Display(Name ="Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Parse(DateTime.Now.ToString("G"));
        public decimal Amount { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "You have to select a Category")]
        [Display(Name = "Categories")]
        public int IdCategory { get; set; }
        [StringLength(maximumLength:1000, ErrorMessage ="The field {0} cannot be longer than {1}")]
        public string Notes { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "You have to select an Account")]
        [Display(Name = "Accounts")]
        public int IdAccount { get; set; }
        [Display(Name = "Transaction Type")]
        public OperationType IdTransactionType { get; set; } = OperationType.Income;

        public string Accounts { get; set; }
        public string Category { get; set; }

    }
}
