using BudgetManager.Models;
using BudgetManager.Services;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace BudgetManager.Controllers
{
    public class AccountTypesController : Controller
    {
        private readonly IRepositoryAccountTypes repositoryAccountTypes;
        private readonly IRepositoryUsers repositoryUsers;

        public AccountTypesController(IRepositoryAccountTypes repositoryAccountTypes, IRepositoryUsers repositoryUsers)
        {
            this.repositoryAccountTypes = repositoryAccountTypes;
            this.repositoryUsers = repositoryUsers;
        }

        public async Task<IActionResult> Index()
        {

            var idUser = repositoryUsers.GetUserId();
            var list = await repositoryAccountTypes.Get(idUser);
            return View(list);

        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<ActionResult> Edit(int id)
        {

            var idUser = repositoryUsers.GetUserId();
            var accountType = await repositoryAccountTypes.GetById(id, idUser);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(accountType);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AccountType accountType)
        {

            var idUser = repositoryUsers.GetUserId();
            var accountTypeExists = await repositoryAccountTypes.GetById(accountType.Id, idUser);
            if (accountTypeExists is null)
                return RedirectToAction("NotFound", "Home");
            await repositoryAccountTypes.Update(accountType);
            return RedirectToAction("Index", "AccountTypes");

        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountType account)
        {
            if (!ModelState.IsValid)
            {
                return View(account);
            }
            account.IdUser = repositoryUsers.GetUserId();
            var exists = await repositoryAccountTypes.Exists(account.Name, account.IdUser);

            if (exists)
            {
                ModelState.AddModelError(nameof(account.Name), $"The name {account.Name} already exists.");
                return View(account);
            }
            await repositoryAccountTypes.Insert(account);
            return RedirectToAction("Index");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyExist([Bind(Prefix = "AccountType.Name")] string Name, int id)
        {

            var idUser = repositoryUsers.GetUserId();
            var exists = await repositoryAccountTypes.Exists(Name, idUser, id);
            if (exists)
            {
                return Json($"The name {Name} already exists");
            }
            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            var idUser = repositoryUsers.GetUserId();
            var accountType = await repositoryAccountTypes.GetById(id, idUser);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteById(int id)
        {
            var idUser = repositoryUsers.GetUserId();
            var accountType = await repositoryAccountTypes.GetById(id, idUser);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await repositoryAccountTypes.Delete(id);
            return RedirectToAction("Index", "AccountTypes");

        }

        [HttpPost]
        public async Task<IActionResult> Sort([FromBody] int[] ids)
        {

            var idUser = repositoryUsers.GetUserId();
            var accountTypes = await repositoryAccountTypes.Get(idUser);

            var idAccountType = accountTypes.Select(a => a.Id);

            var accountsDoesNotBelongsToUser = ids.Except(idAccountType).ToList();

            if (accountsDoesNotBelongsToUser.Count > 0)
            {
                return Forbid();
            }

            var accountTypesSorted = ids.Select((id, index) => 
            new AccountType(){ Id = id, Position = index + 1 }).AsEnumerable();

            await repositoryAccountTypes.Sort(accountTypesSorted);
            return Ok();
        }
    }
}
