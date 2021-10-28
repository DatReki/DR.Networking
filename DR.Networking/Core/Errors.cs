using System;
using System.Collections.Generic;
using System.Text;

namespace DR.Networking.Core
{
	internal class Errors
	{
		[Serializable]
		public class HostNameTypeNotSupported : Exception
		{
			public HostNameTypeNotSupported() { }

			public HostNameTypeNotSupported(string message) : base(message) { }
		}
	}
}
