using System;

namespace DR.Networking.Models
{
    internal class CheckUrlModel
    {
        public CheckUrlModel()
        {
            Success = false;
            Url = new Uri("about:blank");
            Error = string.Empty;
        }

        public CheckUrlModel(bool? success, Uri? url, string? error)
        {
            Success = success ?? false;
            Url = url ?? new Uri("about:blank");
            Error = error ?? string.Empty;
        }

        /// <summary>
        /// Indicates if the provided URL was valid or not.
        /// </summary>
        internal bool Success { get; set; }

        /// <summary>
        /// The resulting Uri if <see cref="Success"/> is true.
        /// </summary>
        internal Uri Url { get; set; }

        /// <summary>
        /// An error message explaining what went wrong if <see cref="Success"/> is false.
        /// </summary>
        internal string Error { get; set; }
    }
}
