using System;

namespace Tojeero.Core
{
	public class AuthenticationException : Exception
	{
		#region Properties

		public UnauthorizedReason Reason { get; set; }

		#endregion

		#region Constructors

		public AuthenticationException()
		{
			
		}

		public AuthenticationException(UnauthorizedReason reason)
		{
			Reason = reason;
		}

		#endregion
	}
}

