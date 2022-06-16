using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Services
{
    public class GenerateLinks
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public GenerateLinks(IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }
        
        private IUrlHelper BuildURLHelper()
        {
            var factory = _httpContextAccessor.HttpContext.
                RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        private async Task<bool> IsAdmin()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var result = await _authorizationService.AuthorizeAsync(
                httpContext.User, "isAdmin");

            return result.Succeeded;
        }

        public async Task ToGenerateLinks(AuthorDTO author)
        {
            var isAdmin = await IsAdmin();
            var Url = BuildURLHelper();

            author.Links.Add(new DatoHATEOAS(Url.Link("getAuthor", new { id = author.Id }), "self", "GET"));
            if (isAdmin)
            {
                author.Links.Add(new DatoHATEOAS(Url.Link("updateAuthor", new { id = author.Id }), "update-author", "PUT"));
                author.Links.Add(new DatoHATEOAS(Url.Link("deleteAuthor", new { id = author.Id }), "delete-author", "DELETE"));
            }
        }
    }
}
