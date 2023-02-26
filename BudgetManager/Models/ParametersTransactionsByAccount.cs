namespace BudgetManager.Models
{
    public class ParametersTransactionsByAccount
    {
        public int IdAccount { get; set; }

        public int IdUser { get; set; }
        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

    }
}


