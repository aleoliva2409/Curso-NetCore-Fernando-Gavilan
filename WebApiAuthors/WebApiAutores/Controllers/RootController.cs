using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        
        [HttpGet(Name = "getRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();

            var isAdmin = await _authorizationService.AuthorizeAsync(User, "isAdmin");

            datosHateoas.Add(new DatoHATEOAS(Url.Link("getRoot", new {}), "self", "GET"));
            datosHateoas.Add(new DatoHATEOAS(Url.Link("getAuthors", new {}), "authors", "GET"));
            if (isAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOAS(Url.Link("createAuthor", new { }), "create-author", "POST"));
                datosHateoas.Add(new DatoHATEOAS(Url.Link("createBook", new { }), "create-book", "POST"));
            }

            return datosHateoas;
        }
    }
}
