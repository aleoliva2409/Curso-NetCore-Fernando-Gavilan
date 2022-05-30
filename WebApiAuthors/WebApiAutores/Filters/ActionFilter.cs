using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAuthors.Filters
{
    public class ActionFilter : IActionFilter
    {
        private readonly ILogger<ActionFilter> _logger;

        public ActionFilter(ILogger<ActionFilter> logger)
        {
            _logger = logger;
        }

        // en ejecucion
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("Antes de ejecutar la acción");
        }

        // despues de ejecutar
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("Después de ejecutar la acción");

        }
    }
}