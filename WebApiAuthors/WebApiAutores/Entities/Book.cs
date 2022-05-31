using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(maximumLength: 70, ErrorMessage = "Field {0} must be maximum {1} characters")]
        [CapitalLetter]
        public string Title { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
