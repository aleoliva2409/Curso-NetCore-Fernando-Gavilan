using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Author : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "Field {0} must be maximum {1} characters")]
        [CapitalLetter] // custom validation
        public string Name { get; set; }

        /*
        [Range(18, 120, ErrorMessage = "Field {0} must be between {1} and {2}")]
        [NotMapped] // No la mapea como columna de la tabla
        public int Age { get; set; }

        [CreditCard]
        [NotMapped]
        public string CreditCard { get; set; }

        [Url]
        [NotMapped]
        public string  UrlBiography { get; set; }
        
        */
        
        public List<Book> Books { get; set; }

        // Solo se ejecuta si paso todas las validaciones anteriores
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var firstLetter = Name.Substring(0, 1);

                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult("Field {0} must start with a capital letter", new[] { "Name" });
                }
            }
        }
    }
}
