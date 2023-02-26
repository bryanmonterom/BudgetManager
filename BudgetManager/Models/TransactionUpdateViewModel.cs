namespace BudgetManager.Models
{
    public class TransactionUpdateViewModel : TransactionCreationViewModel
    {
        public int PreviousAccountId { get; set; }
        public decimal PreviousAmount { get; set; }
        public string ReturnURL { get; set; }
    }
}
