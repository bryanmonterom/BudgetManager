using BudgetManager.Models;

namespace BudgetManager.Services
{
    public interface IReportService
    {
        Task<IEnumerable<ResultsGetByWeek>> GetByWeeks(int idUser, int year, int month, dynamic ViewBag);
        Task<TransactionDetailViewModel> GetTransactionsDetailByAccount(int id, int idUser, int month, int year, dynamic ViewBag);
        Task<TransactionDetailViewModel> GetTransactionsDetailByUser(int idUser, int month, int year, dynamic ViewBag);
    }
    public class ReportService : IReportService
    {
        private readonly IRepositoryTransactions repositoryTransactions;
        private readonly HttpContext httpContext;

        public ReportService
            (IRepositoryTransactions repositoryTransactions, IHttpContextAccessor contextAccessor)
        {
            this.repositoryTransactions = repositoryTransactions;
            this.httpContext = contextAccessor.HttpContext;
        }
        public async Task<TransactionDetailViewModel> GetTransactionsDetailByAccount(int id, int idUser, int month, int year, dynamic ViewBag)
        {
            (DateTime dateStart, DateTime dateEnd) = GenerateDates(month, year);

            var getTransactionsByAccount = new ParametersTransactionsByAccount()
            {
                IdAccount = id,
                IdUser = idUser,
                DateEnd = dateEnd,
                DateStart = dateStart,

            };

            var transactions = await repositoryTransactions.GetByAccountId(getTransactionsByAccount);

            TransactionDetailViewModel model = GenerateTransactionsDetail(dateStart, dateEnd, transactions);


            AssignValuesToViewBag(ViewBag, dateStart);
            return model;

        }

        public async Task<TransactionDetailViewModel> GetTransactionsDetailByUser(int idUser, int month, int year, dynamic ViewBag)
        {
            (DateTime dateStart, DateTime dateEnd) = GenerateDates(month, year);

            var getTransactionsByUser = new ParametersTransactionsByUsers()
            {
                IdUser = idUser,
                DateEnd = dateEnd,
                DateStart = dateStart,

            };

            var transactions = await repositoryTransactions.GetByIdUser(getTransactionsByUser);

            TransactionDetailViewModel model = GenerateTransactionsDetail(dateStart, dateEnd, transactions);

            AssignValuesToViewBag(ViewBag, dateStart);
            return model;

        }

        public async Task<IEnumerable<ResultsGetByWeek>> GetByWeeks(int idUser, int year, int month, dynamic ViewBag) { 

            (DateTime dateStart, DateTime dateEnd) = GenerateDates((int)month, (int)year);

            var getTransactionsByUser = new ParametersTransactionsByUsers()
            {
                IdUser = idUser,
                DateEnd = dateEnd,
                DateStart = dateStart,

            };

            AssignValuesToViewBag(ViewBag, dateStart);
            var model = await repositoryTransactions.GetByWeek(getTransactionsByUser);
            return model;


        }

        private void AssignValuesToViewBag(dynamic ViewBag, DateTime dateStart)
        {
            ViewBag.PreviousMonth = dateStart.AddMonths(-1).Month;
            ViewBag.PreviousYear = dateStart.AddMonths(-1).Year;
            ViewBag.NextMonth = dateStart.AddMonths(1).Month;
            ViewBag.NextYear = dateStart.AddMonths(1).Year;
            ViewBag.ReturnURL = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static TransactionDetailViewModel GenerateTransactionsDetail(DateTime dateStart, DateTime dateEnd, IEnumerable<Transaction> transactions)
        {
            var model = new TransactionDetailViewModel();

            var transactionsByDate = transactions.OrderBy(a => a.TransactionDate).
                                                  GroupBy(a => a.TransactionDate).
                                                  Select(a => new TransactionDetailViewModel.TransactionsByDate()
                                                  {
                                                      TransactionDate = a.Key,
                                                      Transactions = a.AsEnumerable()
                                                  });

            model.TransactionsByDates = transactionsByDate;
            model.DateStart = dateStart;
            model.DateEnd = dateEnd;
            return model;
        }

        private (DateTime dateStart, DateTime dateEnd) GenerateDates(int month, int year)
        {
            DateTime dateStart;
            DateTime dateEnd;

            if (month <= 0 || month > 12 || year <= 1900)
            {

                var today = DateTime.Today;
                dateStart = new DateTime(today.Year, today.Month, 1);
            }
            else
            {

                dateStart = new DateTime((int)year, (int)month, 1);
            }
            dateEnd = dateStart.AddMonths(1).AddDays(-1);

            return (dateStart, dateEnd);
        }
    }
}
