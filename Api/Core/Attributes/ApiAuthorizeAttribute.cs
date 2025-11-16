using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Attributes
{
    public class ApiAuthorizeAttribute : TypeFilterAttribute
    {
        public ApiAuthorizeAttribute() : base(typeof(ApiAuthorizeFilter))
        {
        }
    }
}
