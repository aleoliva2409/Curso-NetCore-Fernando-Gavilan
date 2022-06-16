using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebApiAuthors.Utils
{
    public class HeaderAttribute : Attribute, IActionConstraint
    {
        private readonly string _headerName;
        private readonly string _headerValue;

        public HeaderAttribute(string headerName, string headerValue)
        {
            _headerName = headerName;
            _headerValue = headerValue;
        }
        
        public int Order => 0;
        public bool Accept(ActionConstraintContext context)
        {
            var headers = context.RouteContext.HttpContext.Request.Headers;

            if (!headers.ContainsKey(_headerName))
            {
                return false;
            }

            return string.Equals(headers[_headerName], _headerValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
