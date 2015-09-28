using System;

namespace Tojeero.Core
{
	public static class TraceMessages
	{
		public static string OperationTimeOutNoNetwork = "Operation timed out. Seems that it was due to the lack of network connection.";
		public static string OperationTimeOut = "Operation timed out because of unknown reason.";

		public static class Auth
		{
			public static string Unknown = "Authentication failed because of unknown error. {0}";
			public static string BadHttpStatusCode = "Authentication failed because of bad HTTP status code. {0}";
			public static string WebException = "Authentication failed because of network issue. {0}";

			public static string FacebookAuthCancelled = "Facebook authentication has been cancelled.";
			public static string FacebookAuthError = "Error occurred while facebook authentication. {0}";
			public static string FacebookAuthSuccess = "Successful facebook authentication. Access token is {0}";
		}
	}
}

