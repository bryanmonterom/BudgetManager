namespace BudgetManager.Models
{
    public class ResultsGetByMonth
    {
        public int Month { get; set; }
        public DateTime DateReference { get; set; }
        public decimal  Amount { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public OperationType IdTransactionType { get; set; }
    }
}
