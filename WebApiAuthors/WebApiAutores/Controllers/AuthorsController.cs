using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    //[Route("api/authors")]
    [Route("api/[controller]")] // [controller] = Authors // lleva el nombre del controlador
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthorsController(AppDbContext context)
        {
            _context = context;
        }

        // puedo poner varias rutas para un metodo
        [HttpGet] // api/authors
        [HttpGet("listado")] // api/authors/listado
        [HttpGet("/listado")] // listado
        public async Task<ActionResult<List<Author>>> Get()
        {
            return await _context.Authors.Include(a =>
                a.Books).ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> Get([FromRoute] int id) // especificamos de donde viene la data
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Author>> Get([FromRoute] string name)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name.Contains(name));

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        [HttpGet("testBinding")]
        public void Get([FromHeader] string token, [FromQuery] string q ) // especificamos de donde viene la data
        {
            Console.WriteLine(token);
            Console.WriteLine(q);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Author author)
        {
            var authorExists = await _context.Authors.AnyAsync(a => a.Name == author.Name);

            if (authorExists)
            {
                return BadRequest($"Author {author.Name} already exists");
            }

            _context.Add(author);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Author author)
        {
            if (author.Id != id)
            {
                return NotFound();
            }

            var isExistAuthor = await _context.Authors.AnyAsync(a => a.Id == id);

            if (!isExistAuthor)
            {
                return NotFound();
            }


            _context.Update(author);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            // var author = await _context.Authors.FindAsync(id);
            var author = await _context.Authors.AnyAsync(a => a.Id == id);

            if (!author)
            {
                return NotFound();
            }
            _context.Remove(new Author() { Id = id }); //debemos mandarle un objeto con el id del author a eliminar
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
