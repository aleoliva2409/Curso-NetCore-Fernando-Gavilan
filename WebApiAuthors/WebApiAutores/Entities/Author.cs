using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "Field {0} must be maximum {1} characters")]
        [CapitalLetter] // custom validation
        public string Name { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }
    }
}
