namespace WebApiAuthors.Middlewares
{
    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggerMiddlewareResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggerMiddleware>();
        }
    }

    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggerMiddleware> _logger;

        public LoggerMiddleware(RequestDelegate next,
            ILogger<LoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var originalBodyRes = context.Response.Body;
                context.Response.Body = ms;

                await _next(context);
                
                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(originalBodyRes);
                context.Response.Body = originalBodyRes;

                _logger.LogInformation(response);
            }
        }
    }
}
