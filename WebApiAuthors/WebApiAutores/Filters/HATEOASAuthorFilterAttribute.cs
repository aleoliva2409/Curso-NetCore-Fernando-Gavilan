using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAuthors.DTOs;
using WebApiAuthors.Services;
using WebApiAuthors.Utils;

namespace WebApiAuthors.Filters
{
    public class HATEOASAuthorFilterAttribute : HATEOASFilterAttribute
    {
        private readonly GenerateLinks _generateLinks;

        public HATEOASAuthorFilterAttribute(GenerateLinks generateLinks)
        {
            _generateLinks = generateLinks;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            var shouldInclude = IncludeHATEOAS(context);

            if (!shouldInclude)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;
            var authorDto = result.Value as AuthorDTO;

            if (authorDto == null)
            {
                var authorsDto = result.Value as List<AuthorDTO> ??
                throw new ArgumentNullException("It should a AuthorDTO or List<AuthorDTO> instance");

                authorsDto.ForEach(async author => await _generateLinks.ToGenerateLinks(author));
                result.Value = authorsDto;
            }
            else
            {
                await _generateLinks.ToGenerateLinks(authorDto);
            }

            await next();
        }
    }
}
