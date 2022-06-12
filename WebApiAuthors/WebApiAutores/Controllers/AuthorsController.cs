﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthorsController(AppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("Configurations")]
        public ActionResult<string> GetConfigurations()
        {
            // toma primero las variables por linea de comando, luego las variables de entorno, despues las de user secrets y por ultimo las de appSettings
            return _configuration["lastname"];

            // otra manera de ingresar dentro un objeto y obtener el valor
            // no es case sensitive
            // return _configuration["connectionStrings:defaultConnection"];
        }

        // puedo poner varias rutas para un metodo
        [HttpGet] // api/authors
        public async Task<ActionResult<List<AuthorDTO>>> Get()
        {
            var authors = await _context.Authors.ToListAsync();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorWithBooksDTO>> Get([FromRoute] int id)
        {
            var author = await _context.Authors
                .Include(authorDB => authorDB.AuthorsBooks)
                .ThenInclude(authorBook => authorBook.Book)
                .FirstOrDefaultAsync(authorDB => authorDB.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return _mapper.Map<AuthorWithBooksDTO>(author);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<List<AuthorDTO>>> Get([FromRoute] string name)
        {
            var authors = await _context.Authors.Where(authorDB => authorDB.Name.Contains(name)).ToListAsync();

            if (authors == null)
            {
                return NotFound();
            }

            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AuthorCreateDTO authorCreateDto)
        {
            var authorExists = await _context.Authors.AnyAsync(a => a.Name == authorCreateDto.Name);

            if (authorExists)
            {
                return BadRequest($"Author {authorCreateDto.Name} already exists");
            }

            var author = _mapper.Map<Author>(authorCreateDto);

            _context.Add(author);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, AuthorCreateDTO authorCreateDto)
        {
           var isExistAuthor = await _context.Authors.AnyAsync(a => a.Id == id);

            if (!isExistAuthor)
            {
                return NotFound();
            }

            var author = _mapper.Map<Author>(authorCreateDto);
            author.Id = id;

            _context.Update(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var authorExist = await _context.Authors.AnyAsync(a => a.Id == id);

            if (!authorExist)
            {
                return NotFound();
            }
            _context.Remove(new Author() { Id = id }); //debemos mandarle un objeto con el id del author a eliminar
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
