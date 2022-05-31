using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _context.Books.Include(bookDB => bookDB.Comments)
                .FirstOrDefaultAsync(b => b.Id == id);

            return _mapper.Map<BookDTO>(book);
        }

        [HttpPost]
        public async Task<ActionResult> Post(BookCreateDTO bookCreateDTO)
        {
            /*var isExistAuthor = await _context.Authors.AnyAsync(a => a.Id == bookCreateDTO.AuthorId);

            if (!isExistAuthor)
            {
                return BadRequest("No existe el Author");
            }*/

            var book = _mapper.Map<Book>(bookCreateDTO);
            _context.Add(book);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
