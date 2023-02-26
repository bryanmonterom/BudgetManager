namespace BudgetManager.Models
{
    public class WeeklyReportViewModel
    {
        public decimal Income => WeeklyTransactions.Sum(x => x.Income);
        public decimal Total => Income - Expenses;
        public DateTime DateReference { get; set; }
        public IEnumerable<ResultsGetByWeek> WeeklyTransactions { get; set; }
        public decimal Expenses=> WeeklyTransactions.Sum(x => x.Expenses); 
    }
}
