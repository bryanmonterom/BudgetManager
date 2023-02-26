using AutoMapper;
using BudgetManager.Models;
using BudgetManager.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BudgetManager.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IRepositoryTransactions repositoryTransactions;
        private readonly IRepositoryUsers repositoryUsers;
        private readonly IRepositoryAccounts repositoryAccounts;
        private readonly IRepositoryCategories repositoryCategories;
        private readonly IMapper mapper;
        private readonly IReportService reportService;

        public TransactionsController(IConfiguration configuration,
            IRepositoryTransactions repositoryTransactions,
            IRepositoryUsers repositoryUsers,
            IRepositoryAccounts repositoryAccounts,
            IRepositoryCategories repositoryCategories, IMapper mapper, IReportService reportService)
        {
            this.configuration = configuration;
            this.repositoryTransactions = repositoryTransactions;
            this.repositoryUsers = repositoryUsers;
            this.repositoryAccounts = repositoryAccounts;
            this.repositoryCategories = repositoryCategories;
            this.mapper = mapper;
            this.reportService = reportService;
        }

        public async Task<IActionResult> Index(int month, int year)
        {
            var idUser = repositoryUsers.GetUserId();
            var model = await reportService.GetTransactionsDetailByUser(idUser, month, year, ViewBag);

            return View(model);
        }
        public async Task<IActionResult> Weekly(int month, int year)
        {

            var idUser = repositoryUsers.GetUserId();

            IEnumerable<ResultsGetByWeek> transactions = await reportService.GetByWeeks(idUser, year, month, ViewBag);


            var group = transactions.GroupBy(a => a.Week).Select(b => new ResultsGetByWeek()
            {

                Week = b.Key,
                Expenses = b.Where(a => a.IdTransactionType == OperationType.Expense).Select(a => a.Amount).FirstOrDefault(),
                Income = b.Where(a => a.IdTransactionType == OperationType.Income).Select(a => a.Amount).FirstOrDefault(),

            }).ToList();

            if (year == 0 || month == 0)
            {

                var today = DateTime.Today;
                year = today.Year;
                month = today.Month;
            }

            var dateReference = new DateTime(year, month, 1);
            var daysOfTheMonth = Enumerable.Range(1, dateReference.AddMonths(1).AddDays(-1).Day);

            var daysSegmented = daysOfTheMonth.Chunk(7).ToList();

            for (int i = 0; i < daysSegmented.Count(); i++)
            {
                var week = i + 1;
                var dateStart = new DateTime(year, month, daysSegmented[i].First());
                var dateEnd = new DateTime(year, month, daysSegmented[i].Last());

                var groupWeek = group.FirstOrDefault(a => a.Week == week);

                if (groupWeek is null)
                {
                    group.Add(new ResultsGetByWeek()
                    {
                        DateStart = dateStart,
                        DateEnd = dateEnd,
                        Amount = 0,
                        Week = week
                    });
                }
                else
                {
                    groupWeek.DateEnd = dateEnd;
                    groupWeek.DateStart = dateStart;
                }
            }


            group = group.OrderByDescending(a => a.Week).ToList();

            var model = new WeeklyReportViewModel()
            {
                WeeklyTransactions = group,
                DateReference = dateReference
            };

            return View(model);
        }
        public IActionResult Calendar()
        {

            return View(SubMenuTransactions.Calendar);
        }

        public async Task<JsonResult> GetCalendarTransactions(DateTime start, DateTime end) { 
            
            var idUser = repositoryUsers.GetUserId();
            var transactions = await repositoryTransactions.GetByIdUser(new ParametersTransactionsByUsers()
            {
                DateEnd = end,
                DateStart = start,
                IdUser = idUser
            });

            var events = transactions.Select(a => new CalendarEvents()
            {
                Title = a.Amount.ToString("N"),
                Start = a.TransactionDate.ToString("yyyy-MM-dd"),
                End = a.TransactionDate.ToString("yyyy-MM-dd"),
                Color = a.IdTransactionType == OperationType.Income ? "Green" : "Red"
            }); 

            return Json(events);


        }
        [HttpGet]
        public async Task<JsonResult> GetTransactionsByDate(DateTime date) {

            var idUser = repositoryUsers.GetUserId();
            var transactions = await repositoryTransactions.GetByIdUser(new ParametersTransactionsByUsers()
            {
                DateEnd = date,
                DateStart = date,
                IdUser = idUser
            });
            return Json(transactions);


        }
        public async Task<IActionResult> Monthly(int year)
        {
            var idUser = repositoryUsers.GetUserId();

            if (year == 0)
            {

                year = DateTime.Today.Year;
            }

            var transactions = await repositoryTransactions.GetByMonths(idUser, year);

            var group = transactions.GroupBy(a => a.Month).Select(a => new ResultsGetByMonth()
            {
                Month = a.Key,
                Income = a.Where(b => b.IdTransactionType == OperationType.Income).Select(a => a.Amount).FirstOrDefault(),
                Expense = a.Where(b => b.IdTransactionType == OperationType.Expense).Select(a => a.Amount).FirstOrDefault(),


            }).ToList();

            for (int i = 1; i <= 12; i++)
            {
                var transaction = group.Where(a => a.Month == i).FirstOrDefault();
                var dateReference = new DateTime(year, i, 1);
                if (transaction is null)
                {
                    group.Add(new ResultsGetByMonth()
                    {
                        DateReference = dateReference,
                        Month = i
                    });
                }
                else
                {
                    transaction.DateReference = dateReference;
                }
            }
            group.OrderByDescending(a => a.Month).ToList();

            var model = new MonthlyReportViewModel()
            {

                Transactions = group,
                Year = year,

            };


            return View(model);
        }
        public IActionResult ExcelReport()
        {

            return View(SubMenuTransactions.Excel);
        }

        public async Task<FileResult> ExcportExcelByMonth(int month, int year)
        {

            var dateStart = new DateTime(year, month, 1);
            var dateEnd = dateStart.AddMonths(1).AddDays(-1);
            var idUser = repositoryUsers.GetUserId();

            var transactions = await repositoryTransactions.GetByIdUser(new ParametersTransactionsByUsers()
            {
                DateEnd = dateEnd,
                DateStart = dateStart,
                IdUser = idUser
            });

            var fileName = $"BudgetManager - {dateStart.ToString("MMM yyyy")}.xlsx";

            return GenerateExcel(fileName, transactions);
        }

        public async Task<FileResult> ExportExcelByYear(int year) {

            var idUser = repositoryUsers.GetUserId();
            var dateStart = new DateTime(year, 1, 1);
            var dateEnd = dateStart.AddYears(1).AddDays(-1);

            var transactions = await repositoryTransactions.GetByIdUser(new ParametersTransactionsByUsers()
            {
                DateEnd = dateEnd,
                DateStart = dateStart,
                IdUser = idUser
            });

            var fileName = $"BudgetManager - {dateStart.ToString("yyyy")}.xlsx";

            return GenerateExcel(fileName, transactions);

        }

        public async Task<FileResult> ExportExcelEverything() {

            var dateStart = DateTime.Today.AddYears(-100);
            var dateEnd = dateStart.AddYears(1000);
            var idUser = repositoryUsers.GetUserId();

            var transactions = await repositoryTransactions.GetByIdUser(new ParametersTransactionsByUsers()
            {
                DateEnd = dateEnd,
                DateStart = dateStart,
                IdUser = idUser
            });

            var fileName = $"BudgetManager - {DateTime.Today.ToString("dd-MM-yyyy")}.xlsx";

            return GenerateExcel(fileName, transactions);
        }

        private FileResult GenerateExcel(string fileName, IEnumerable<Transaction> transactions)
        {

            DataTable dt = new DataTable("Transactions");

            dt.Columns.AddRange(new DataColumn[] {
            new DataColumn("Date"),
            new DataColumn("Account"),
            new DataColumn("Category"),
            new DataColumn("Amount"),
            new DataColumn("Notes"),
            new DataColumn("Income/Expense")
            });

            foreach (var item in transactions)
            {
                dt.Rows.Add(item.TransactionDate, item.Accounts, item.Category, item.Amount, item.Notes, item.IdTransactionType);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", fileName);
                }

            }
        }
        public async Task<IActionResult> Create()
        {
            var idUser = repositoryUsers.GetUserId();
            var model = new TransactionCreationViewModel()
            {
                Accounts = await GetAccountListItems(idUser)
            };
            model.Categories = await GetCategoriesListItems(idUser, model.IdTransactionType);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreationViewModel transactionCreation)
        {

            var idUser = repositoryUsers.GetUserId();

            if (!ModelState.IsValid)
            {

                var model = new TransactionCreationViewModel()
                {
                    Accounts = await GetAccountListItems(idUser)
                };
                model.Categories = await GetCategoriesListItems(idUser, model.IdTransactionType);
                return View(model);
            }

            var account = await repositoryAccounts.GetById(transactionCreation.IdAccount, idUser);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = repositoryCategories.GetById(transactionCreation.IdCategory, idUser);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            transactionCreation.IdUser = idUser;

            transactionCreation.Amount = transactionCreation.IdTransactionType == OperationType.Income ? transactionCreation.Amount : transactionCreation.Amount * -1;

            await repositoryTransactions.Create(transactionCreation);


            return RedirectToAction("Index");

        }

        private async Task<IEnumerable<SelectListItem>> GetAccountListItems(int idUser)
        {
            var account = await repositoryAccounts.GetAll(idUser);
            return account.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).OrderBy(a => a.Text);
        }

        private async Task<IEnumerable<SelectListItem>> GetCategoriesListItems(int idUser, OperationType operation)
        {
            var account = await repositoryCategories.GetAll(idUser, operation);
            var defaultOption = new SelectListItem("Select a category", "0", true);
            var results = account.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).OrderBy(a => a.Text).ToList();
            results.Insert(0, defaultOption);
            return results;
        }

        [HttpPost]
        public async Task<IActionResult> GetCategories([FromBody] OperationType operationType)
        {
            var idUser = repositoryUsers.GetUserId();
            var categories = await GetCategoriesListItems(idUser, operationType);
            return Ok(categories);
        }

        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {

            var idUser = repositoryUsers.GetUserId();
            var transaction = await repositoryTransactions.GetById(id, idUser);

            if (transaction == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = mapper.Map<TransactionUpdateViewModel>(transaction);
            transaction.Amount = transaction.IdTransactionType == OperationType.Income ? transaction.Amount : transaction.Amount * -1;

            model.PreviousAccountId = transaction.IdAccount;
            model.PreviousAmount = transaction.Amount;
            model.Categories = await GetCategoriesListItems(idUser, transaction.IdTransactionType);
            model.Accounts = await GetAccountListItems(idUser);
            model.ReturnURL = returnUrl;
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(TransactionUpdateViewModel model)
        {

            var idUser = repositoryUsers.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoriesListItems(idUser, model.IdTransactionType);
                model.Accounts = await GetAccountListItems(idUser);
                return View(model);
            }

            var account = await repositoryAccounts.GetById(model.IdAccount, idUser);
            if (account == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = await repositoryCategories.GetById(model.IdCategory, idUser);
            if (category == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var transaction = mapper.Map<Transaction>(model);

            transaction.Amount = transaction.IdTransactionType == OperationType.Income ? transaction.Amount : transaction.Amount * -1;

            await repositoryTransactions.Update(transaction, previousAccount: model.PreviousAccountId, previousAmount: model.PreviousAmount);

            if (string.IsNullOrEmpty(model.ReturnURL))
            {

                return RedirectToAction("Index");

            }
            return LocalRedirect(model.ReturnURL);


        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {

            var idUser = repositoryUsers.GetUserId();
            var transaction = repositoryTransactions.GetById(id, idUser);


            if (transaction == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await repositoryTransactions.Delete(id);

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");

            }

            return LocalRedirect(returnUrl);



        }


    }
}
