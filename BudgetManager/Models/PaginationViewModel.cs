namespace BudgetManager.Models
{
    public class PaginationViewModel
    {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private readonly int maxRecordsPerPage = 50;

        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set { recordsPerPage = (value > maxRecordsPerPage) ? maxRecordsPerPage : value; }


        }

        public int RecordsToIgnore => recordsPerPage * (Page - 1);
    }
}
