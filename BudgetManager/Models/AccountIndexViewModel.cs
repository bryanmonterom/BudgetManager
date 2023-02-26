namespace BudgetManager.Models
{
    public class AccountIndexViewModel
    {
        public string AccountType { get; set; }
        public IEnumerable<Account> Accounts { get; set;  }
        public decimal TotalBalance => Accounts.Sum(x => x.Balance);
        public string Icon { get { return GetIcon(this.AccountType); } }

        public string GetIcon(string name)
        {
            var defaultIcon = "bi bi-wallet";
            var iconLists = new List<IconsByAccounts>(){

                new IconsByAccounts(){Name ="Cash",IconName="bi bi-cash" },
                new IconsByAccounts(){Name ="Savings Accounts",IconName="bi bi-piggy-bank-fill" },
                new IconsByAccounts(){Name ="Credit Cards",IconName="bi bi-credit-card-fill" },
                new IconsByAccounts(){Name ="General",IconName="bi bi-wallet" },
                new IconsByAccounts(){Name ="Investments",IconName="bi bi-graph-up" },
                new IconsByAccounts(){Name ="Loan",IconName="bi bi-bank2" },
                new IconsByAccounts(){Name ="Mortage",IconName="bi bi-house-lock-fill" },
                new IconsByAccounts(){Name ="Bonus",IconName="bi bi-cash-coin" },
                new IconsByAccounts(){Name ="Credit Cards",IconName="bi bi-credit-card-fill" },
                new IconsByAccounts(){Name ="Insurance",IconName=" bi bi-shield-check" }






            };
            var iconName = iconLists.Where(a => a.Name == name).FirstOrDefault();
            return iconName is not null? iconName.IconName: defaultIcon;
        }
    }

    
}
