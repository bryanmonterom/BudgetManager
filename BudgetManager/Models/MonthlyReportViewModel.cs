namespace BudgetManager.Models
{
    public class MonthlyReportViewModel
    {
        public IEnumerable<ResultsGetByMonth> Transactions { get; set; }
        public decimal Income => Transactions.Sum(a => a.Income);
        public decimal Expenses => Transactions.Sum(a => a. Expense);
        public decimal Total => Income - Expenses;
        public int Year { get; set; }   

    }
}
