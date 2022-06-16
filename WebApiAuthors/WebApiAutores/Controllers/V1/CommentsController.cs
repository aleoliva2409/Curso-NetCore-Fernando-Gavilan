using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;
using WebApiAuthors.Utils;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    [Route("api/v1/books/{bookId:int}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(AppDbContext context, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // cambiamos para usar pagination
        [HttpGet(Name = "getComments")]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId,
            [FromQuery] PaginationDTO paginationDTO)
        {
            var bookExist = await _context.Books.AnyAsync(bookDB => bookDB.Id == bookId);

            if (!bookExist)
            {
                return BadRequest("No existe el Libro");
            }
            var queryable = _context.Comments.Where(commentDB => commentDB.BookId == bookId).AsQueryable();
            await HttpContext.InsertPaginationParametersInHeaders(queryable);
            var comments = await queryable.OrderBy(comment => comment.Id).ToPaginate(paginationDTO).ToListAsync();

            return _mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpGet("{id:int}", Name = "getComment")]
        public async Task<ActionResult<CommentDTO>> GetById(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(commentDB => commentDB.Id == id);

            if (comment == null)
            {
                return NotFound("No existe el Comentario");
            }
            
            return _mapper.Map<CommentDTO>(comment);
        }

        [HttpPost(Name = "createComment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int bookId, CommentCreateDTO commentCreateDTO)
        {
            // HttpContext funciona si tiene el Authorize, de lo contrario no funciona.
            var email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(email);
            var userId = user.Id;
            var bookExist = await _context.Books.AnyAsync(bookDB => bookDB.Id == bookId);

            if (!bookExist)
            {
                return BadRequest("No existe el Libro");
            }

            var comment = _mapper.Map<Comment>(commentCreateDTO);
            comment.BookId = bookId;
            comment.UserId = userId;
            _context.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("getComment", new { id = comment.Id, bookId = bookId }, _mapper.Map<CommentDTO>(comment));
        }

        [HttpPut("{id:int}", Name = "updateComment")]
        public async Task<ActionResult> Put(int id, int bookId, CommentCreateDTO commentCreateDto)
        {
            var bookExist = await _context.Books.AnyAsync(bookDB => bookDB.Id == bookId);

            if (!bookExist)
            {
                return NotFound();
            }


            var commentExist = await _context.Comments.AnyAsync(commentDB => commentDB.Id == id);

            if (!commentExist)
            {
                return NotFound();
            }

            var comment = _mapper.Map<Comment>(commentCreateDto);
            comment.Id = id;
            comment.BookId = bookId;
            _context.Update(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
