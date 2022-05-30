using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        /*[HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            // Sin la data de Author
            // return await _context.Books.FirstOrDefaultAsync(b => b.Id == id);

            // Con la data de Author
            return await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Book book)
        {
            var isExistAuthor = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);

            if (!isExistAuthor)
            {
                return BadRequest("No existe el Author");
            }

            _context.Add(book);
            await _context.SaveChangesAsync();
            return Ok();
        }*/
    }
}
