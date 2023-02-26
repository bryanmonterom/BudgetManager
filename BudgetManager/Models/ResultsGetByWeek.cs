namespace BudgetManager.Models
{
    public class ResultsGetByWeek
    {
        public int Week { get; set; }
        public decimal Amount { get; set; }
        public OperationType IdTransactionType { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }


    }
}
