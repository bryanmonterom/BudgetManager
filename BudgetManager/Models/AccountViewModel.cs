using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManager.Models
{
    public class AccountViewModel:Account
    {
        public IEnumerable<SelectListItem> AccountTypes { get; set; }   
    }
}
