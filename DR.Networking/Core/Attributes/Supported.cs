using System;

namespace DR.Networking.Core.Attributes
{
    internal class Supported(bool isSupported) : Attribute
    {
        public bool IsSupported = isSupported;
    }
}
