using BudgetManager.Models;
using BudgetManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IRepositoryCategories repositoryCategories;
        private readonly IRepositoryUsers repositoryUsers;

        public CategoriesController(IRepositoryCategories repositoryCategories, IRepositoryUsers repositoryUsers)
        {
            this.repositoryCategories = repositoryCategories;
            this.repositoryUsers = repositoryUsers;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PaginationViewModel pagination)
        {
            var idUser = repositoryUsers.GetUserId();
            var totalRecords = await repositoryCategories.Count(idUser);
            var categories = await repositoryCategories.GetAll(idUser, pagination);
            var paginationVM = new PaginationResponse<Category>()
            {
                Items = categories,
                Page = pagination.Page,
                RecordsPerPage = pagination.RecordsPerPage,
                RecordsTotal = totalRecords,
                BaseUrl = Url.Action()+'/'
            };

            return View(paginationVM);
        }
      

        public IActionResult Create(int id)
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var idUser = repositoryUsers.GetUserId();
            category.IdUser = idUser;
            await repositoryCategories.Create(category);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {

            var idUser = repositoryUsers.GetUserId();
            var category = await repositoryCategories.GetById(id, idUser);

            if (category is null)
            {

                return RedirectToAction("NotFound", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categoryToEdit)
        {

            if (!ModelState.IsValid)
            {
                return View(categoryToEdit);
            }

            var idUser = repositoryUsers.GetUserId();
            var category = await repositoryCategories.GetById(categoryToEdit.Id, idUser);

            if (category is null)
            {

                return RedirectToAction("NotFound", "Home");
            }
            categoryToEdit.IdUser = idUser;
            await repositoryCategories.Update(categoryToEdit);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id) {
            var idUser = repositoryUsers.GetUserId();
            var category = await repositoryCategories.GetById(id, idUser);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var idUser = repositoryUsers.GetUserId();
            var category = await repositoryCategories.GetById(id, idUser);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            await repositoryCategories.Delete(id);
            return RedirectToAction("Index");
        }
    }

}
