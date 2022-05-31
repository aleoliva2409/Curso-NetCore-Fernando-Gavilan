using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CommentsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
        {
            var bookExist = await _context.Books.AnyAsync(bookDB => bookDB.Id == bookId);

            if (!bookExist)
            {
                return BadRequest("No existe el Libro");
            }

            var comments = await _context.Comments.Where(commentDB => commentDB.BookId == bookId).ToListAsync();

            return _mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CommentCreateDTO commentCreateDTO)
        {
            var bookExist = await _context.Books.AnyAsync(bookDB => bookDB.Id == bookId);

            if (!bookExist)
            {
                return BadRequest("No existe el Libro");
            }

            var comment = _mapper.Map<Comment>(commentCreateDTO);
            comment.BookId = bookId;
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
