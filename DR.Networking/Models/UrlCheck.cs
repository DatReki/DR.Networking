using System;

namespace DR.Networking.Models
{
    internal class UrlCheck
    {
        internal UrlCheck()
        {
            Success = false;
            Url = new Uri("about:blank");
            Error = string.Empty;
            ErrorType = 0;
        }

        internal UrlCheck(bool? success, Uri? url, string? error, ErrorType? type)
        {
            Success = success ?? false;
            Url = url ?? new Uri("about:blank");
            Error = error ?? string.Empty;
            ErrorType = type ?? 0;
        }

        internal UrlCheck(UrlCheck original)
        {
            Success = original.Success;
            FromHistory = original.FromHistory;
            Url = original.Url;
            Error = original.Error;
            ErrorType = original.ErrorType;
        }

        /// <summary>
        /// Indicates if the provided URL was valid or not.
        /// </summary>
        internal bool Success { get; set; }

        /// <summary>
        /// If the URL was found in the <see cref="Core.History.Urls"/> list this will be true, otherwise it will be false.
        /// </summary>
        internal bool FromHistory { get; set; } = false;

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
