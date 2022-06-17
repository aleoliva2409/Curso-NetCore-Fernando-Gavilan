using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Validations
{
    public class CapitalLetterAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Console.WriteLine("value: " + value);

            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            if (value.ToString().Length == 0)
            {
                return ValidationResult.Success;
            }

            var firstLetter = value.ToString()[0].ToString();

            if (firstLetter == firstLetter.ToUpper())
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("First letter must be capital");
        }
    }
}
