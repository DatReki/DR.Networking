using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics.CodeAnalysis;

namespace Api.Core.Attributes
{
    /// <summary>
    /// Identifies an action that supports the HTTP TRACE method.
    /// </summary>
    public class HttpTraceAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = ["TRACE"];

        /// <summary>
        /// Creates a new <see cref="HttpTraceAttribute"/>.
        /// </summary>
        public HttpTraceAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Creates a new <see cref="HttpTraceAttribute"/> with the given route template.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpTraceAttribute([StringSyntax("Route")] string template)
            : base(_supportedMethods, template)
        {
            ArgumentNullException.ThrowIfNull(template);
        }
    }
}
