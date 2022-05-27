using Microsoft.AspNetCore.Mvc;
using WebApiAuthorsNet5.Entities;
// Net 5
using System.Collections.Generic;

namespace WebApiAuthorsNet5.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Author>> Get()
        {
            return new List<Author>()
            {
                new Author() { Id = 1, Name = "Alejandro" },
                new Author() { Id = 2, Name = "Daniel" },
            };
        }
    }
}
