using BudgetManager.Models;
using AutoMapper;

namespace BudgetManager.Services
{
    public class AutoMapperProfiles:AutoMapper.Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Account, AccountViewModel>();
            CreateMap<TransactionUpdateViewModel, Transaction>().ReverseMap();
        }
    }
}
