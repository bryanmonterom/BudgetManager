namespace BudgetManager.Models
{
    public class PaginationResponse
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public int RecordsTotal { get; set; }
        public int PagesTotal => (int)Math.Ceiling((double)RecordsTotal / RecordsPerPage);
        public string BaseUrl { get; set; }
    }

    public class PaginationResponse<T> : PaginationResponse
    { 
        public IEnumerable<T>  Items { get; set; }
    }
}
