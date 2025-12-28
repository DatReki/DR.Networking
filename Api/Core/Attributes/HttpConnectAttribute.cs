using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics.CodeAnalysis;

namespace Api.Core.Attributes
{
    /// <summary>
    /// Identifies an action that supports the HTTP CONNECT method.
    /// </summary>
    public class HttpConnectAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = ["CONNECT"];

        /// <summary>
        /// Creates a new <see cref="HttpConnectAttribute"/>.
        /// </summary>
        public HttpConnectAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Creates a new <see cref="HttpConnectAttribute"/> with the given route template.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpConnectAttribute([StringSyntax("Route")] string template)
            : base(_supportedMethods, template)
        {
            ArgumentNullException.ThrowIfNull(template);
        }
    }
}
