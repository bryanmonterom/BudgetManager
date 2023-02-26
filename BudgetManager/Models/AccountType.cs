using BudgetManager.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Models
{
    public class AccountType : IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [FirstCapsLetter]
        [Remote(action: "VerifyExist", controller: "AccountTypesController", AdditionalFields =nameof(Id)) ]
        public string Name { get; set; }
        public int IdUser { get; set; }
        public int Position { get; set; }
       

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name is not null && Name.Length > 0)
            {
                var firstLetter = Name[0].ToString();

                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult("The first letter needs to be on CAPS", new[] { nameof(Name) });
                }
            }
        }
    }
}
