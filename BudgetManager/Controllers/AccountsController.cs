using AutoMapper;
using BudgetManager.Models;
using BudgetManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManager.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IRepositoryAccountTypes accountTypeRepository;
        private readonly IRepositoryUsers usersRepository;
        private readonly IRepositoryAccounts accountsRepository;
        private readonly IMapper mapper;
        private readonly IRepositoryTransactions repositoryTransactions;
        private readonly IReportService reportService;

        public AccountsController(IRepositoryAccountTypes accountTypeRepository,
            IRepositoryUsers usersRepository, IRepositoryAccounts accountsRepository,
            IMapper mapper, IRepositoryTransactions repositoryTransactions, IReportService reportService)
        {
            this.accountTypeRepository = accountTypeRepository;
            this.usersRepository = usersRepository;
            this.accountsRepository = accountsRepository;
            this.mapper = mapper;
            this.repositoryTransactions = repositoryTransactions;
            this.reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var idUser = usersRepository.GetUserId();
            var accounts = await accountsRepository.GetAll(idUser);
            var group = accounts.GroupBy(a => a.AccountType).ToList();
            var model = accounts.GroupBy(a => a.AccountType)
                                .Select(b => new AccountIndexViewModel
                                {
                                    AccountType = b.Key,
                                    Accounts = b.AsEnumerable()
                                }).ToList();
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var idUser = usersRepository.GetUserId();
            var accountType = await accountTypeRepository.Get(idUser);

            var model = new AccountViewModel();
            model.AccountTypes = await GetSelectListItems(idUser);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountViewModel account)
        {
            var idUser = usersRepository.GetUserId();

            var accountType = await accountTypeRepository.GetById(account.IdAccountType, idUser);
            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!ModelState.IsValid)
            {
                account.AccountTypes = await GetSelectListItems(idUser);
                return View(account);

            }

            await accountsRepository.Create(account);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {

            var idUser = usersRepository.GetUserId();

            var account = await accountsRepository.GetById(id, idUser);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = mapper.Map<AccountViewModel>(account);


            model.AccountTypes = await GetSelectListItems(idUser);
            return View(model);

        }
        private async Task<IEnumerable<SelectListItem>> GetSelectListItems(int idUser)
        {

            var accountType = await accountTypeRepository.Get(idUser);

            return accountType.Select(x => new SelectListItem(x.Name, x.Id.ToString()));



        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountViewModel account)
        {
            var idUser = usersRepository.GetUserId();

            var accountToEdit = await accountsRepository.GetById(account.Id, idUser);
            var accountType = await accountTypeRepository.GetById(account.IdAccountType, idUser);

            if (accountToEdit is null || accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!ModelState.IsValid)
            {

                account.AccountTypes = await GetSelectListItems(idUser);
                return View(account);
            }
            await accountsRepository.Update(account);
            return RedirectToAction("Index", "Accounts");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var idUser = usersRepository.GetUserId();
            var account = await accountsRepository.GetById(id, idUser);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var idUser = usersRepository.GetUserId();
            var account = await accountsRepository.GetById(id, idUser);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await accountsRepository.Delete(id);
            return RedirectToAction("Index", "Accounts");
        }

        public async Task<IActionResult> Detail(int id, int month, int year)
        {
            var idUser = usersRepository.GetUserId();

            var account = await accountsRepository.GetById(id, idUser);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            ViewBag.Account = account.Name;
            var model = await reportService.GetTransactionsDetailByAccount(id, idUser, month, year, ViewBag);
            return View(model);


        }
    }
}
