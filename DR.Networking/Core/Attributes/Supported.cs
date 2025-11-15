using System;

namespace DR.Networking.Core.Attributes
{
    internal class Supported : Attribute
    {
        public bool IsSupported;

        public Supported(bool isSupported)
            => IsSupported = isSupported;
    }
}
