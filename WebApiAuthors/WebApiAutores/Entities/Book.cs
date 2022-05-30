using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [CapitalLetter]
        public string Title { get; set; }
    }
}
