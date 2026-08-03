using System.Diagnostics;

namespace Api.Core.Middleware
{
    public class RateLimiter(RequestDelegate next, TimeSpan window, int permitLimit)
    {
        private readonly RequestDelegate _next = next;
        private readonly TimeSpan _window = window;
        private readonly int _permitLimit = permitLimit;
        private long _windowStart = Stopwatch.GetTimestamp();
        private int _requestCount;
        private readonly Lock _lock = new();

        public async Task InvokeAsync(HttpContext context)
        {
            long now = Stopwatch.GetTimestamp();
            long elapsedTicks = now - _windowStart;
            long windowTicks = _window.Ticks * Stopwatch.Frequency / TimeSpan.TicksPerSecond;

            lock (_lock)
            {
                if (elapsedTicks >= windowTicks)
                {
                    _windowStart = now;
                    _requestCount = 0;
                }

                if (_requestCount >= _permitLimit)
                {
                    context.Response.StatusCode = 429;
                    return;
                }

                _requestCount++;
            }

            await _next(context);
        }
    }
}
