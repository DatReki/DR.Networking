using System;

namespace DR.Networking.Core
{
    public class Errors
    {
        [Serializable]
        public class GenericInvalidUrlError : Exception
        {
            public GenericInvalidUrlError() { }

            public GenericInvalidUrlError(string message) : base(message) { }
        }

        [Serializable]
        public class RateLimitingUrlNotValid : Exception
        {
            public RateLimitingUrlNotValid() { }

            public RateLimitingUrlNotValid(string message) : base(message) { }
        }
    }
}
