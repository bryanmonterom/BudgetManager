namespace BudgetManager.Models
{
    public class TransactionDetailViewModel
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public IEnumerable<TransactionsByDate> TransactionsByDates { get; set; }    
        public decimal Income  => TransactionsByDates.Sum(x => x.Income);
        public decimal Expenses => TransactionsByDates.Sum(_ => _.Expenses);
        public decimal Total => Income - Expenses;




        public class TransactionsByDate
        {

            public DateTime TransactionDate { get; set; }
            public IEnumerable<Transaction> Transactions { get; set; }
            public decimal Income => 
                Transactions.Where(a => a.IdTransactionType == OperationType.Income)
                            .Sum(a => a.Amount);
            public decimal Expenses =>
              Transactions.Where(a => a.IdTransactionType == OperationType.Expense)
                          .Sum(a => a.Amount);

        }
    }
}
