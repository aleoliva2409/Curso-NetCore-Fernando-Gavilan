using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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

        // Vamos a darle un nombre a esta ruta
        [HttpGet("{id:int}", Name = "getBook")]
        public async Task<ActionResult<BookWithAuthorsDTO>> GetBook(int id)
        {
            /*var book = await _context.Books.Include(bookDB => bookDB.Comments)
                .FirstOrDefaultAsync(b => b.Id == id);*/

            var book = await _context.Books
                .Include(bookDB => bookDB.AuthorsBooks)
                .ThenInclude(authorBookDB => authorBookDB.Author)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            
            //sirve para ordenarlos y mandarselos asi al front
            book.AuthorsBooks = book.AuthorsBooks.OrderBy(x => x.Order).ToList();

            return _mapper.Map<BookWithAuthorsDTO>(book);
        }

        [HttpPost(Name = "createBook")]
        public async Task<ActionResult> Post(BookCreateDTO bookCreateDTO)
        {
            if (bookCreateDTO.AuthorsIds == null)
            {
                return BadRequest("No se puede crear libro sin Autores");
            }

            var authorsIds = await _context.Authors.Where(authorDB =>
                bookCreateDTO.AuthorsIds.Contains(authorDB.Id)).Select(a => a.Id).ToListAsync();

            if(bookCreateDTO.AuthorsIds.Count != authorsIds.Count)
            {
                return BadRequest("No existe uno de los Autores enviados");
            }

            var book = _mapper.Map<Book>(bookCreateDTO);
            
            if(book.AuthorsBooks != null)
            {
                for (int i = 0; i < book.AuthorsBooks.Count; i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }

            _context.Add(book);
            await _context.SaveChangesAsync();

            // Armamos la response de este POST con buenas practicas
            // se le va a mandar por headers la ruta para consultar el lbiro creado
            var bookDto = _mapper.Map<BookDTO>(book);

            return CreatedAtRoute("getBook", new { id = book.Id }, bookDto);
        }

        [HttpPut("{id:int}", Name = "updateBook")]
        public async Task<ActionResult> Put(int id, BookCreateDTO bookCreateDto)
        {
            var bookDB = await _context.Books.Include(x => x.AuthorsBooks).FirstOrDefaultAsync(x => x.Id == id);

            if (bookDB == null)
            {
                return NotFound();
            }

            bookDB = _mapper.Map(bookCreateDto, bookDB);

            if (bookDB.AuthorsBooks != null)
            {
                for (int i = 0; i < bookDB.AuthorsBooks.Count; i++)
                {
                    bookDB.AuthorsBooks[i].Order = i;
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "patchBook")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> bookPatchDto)
        {
            if (bookPatchDto == null)
            {
                return BadRequest();
            }

            var bookDB = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (bookDB == null)
            {
                return NotFound();
            }
            
            var bookDto = _mapper.Map<BookPatchDTO>(bookDB);

            // hace el patcheo de los campos enviados
            bookPatchDto.ApplyTo(bookDto, ModelState);

            // validamos que los campos que se quieren actualizar no esten vacios
            var isValid = TryValidateModel(bookDto);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(bookDto, bookDB);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "deleteBook")]
        public async Task<ActionResult> Delete(int id)
        {
            var bookExist = await _context.Books.AnyAsync(b => b.Id == id);
            
            if (!bookExist)
            {
                return NotFound();
            }
            _context.Remove(new Book() { Id = id }); //debemos mandarle un objeto con el id del author a eliminar
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
