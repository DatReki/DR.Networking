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
            ErrorType = 0;
        }

        public CheckUrlModel(bool? success, Uri? url, string? error, ErrorType? type)
        {
            Success = success ?? false;
            Url = url ?? new Uri("about:blank");
            Error = error ?? string.Empty;
            ErrorType = type ?? 0;
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

        /// <summary>
        /// Indicates what type of error the error message is referencing.
        /// </summary>
        internal ErrorType ErrorType { get; set; }
    }
}
