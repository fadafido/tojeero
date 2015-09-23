using System;

namespace Tojeero.Core
{
	public static class TraceMessages
	{
		public static class Auth
		{
			public static string Unknown = "Authentication failed because of unknown error. {0}";
			public static string BadHttpStatusCode = "Authentication failed because of bad HTTP status code. {0}";
			public static string WebException = "Authentication failed because of network issue. {0}";
		}
	}
}

