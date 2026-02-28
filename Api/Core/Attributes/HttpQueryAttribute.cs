using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics.CodeAnalysis;

namespace Api.Core.Attributes
{
    /// <summary>
    /// Identifies an action that supports the HTTP QUERY method.
    /// </summary>
    public class HttpQueryAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = ["QUERY"];

        /// <summary>
        /// Creates a new <see cref="HttpTraceAttribute"/>.
        /// </summary>
        public HttpQueryAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Creates a new <see cref="HttpQueryAttribute"/> with the given route template.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpQueryAttribute([StringSyntax("Route")] string template)
            : base(_supportedMethods, template)
        {
            ArgumentNullException.ThrowIfNull(template);
        }
    }
}
