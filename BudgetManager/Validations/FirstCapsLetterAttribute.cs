using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Validations
{
    public class FirstCapsLetterAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString())) {
                return ValidationResult.Success;
            }
            
            var firstLetter =value.ToString()[0].ToString();

            if (firstLetter != firstLetter.ToUpper()) {

                return new ValidationResult("The first letter needs to be on CAPS");
            }


            return ValidationResult.Success;
        }
    }
}
