using System;

namespace Tojeero.Core.Services
{
	public class AuthenticationException : Exception
	{
		public AuthenticationResultCode ResultCode { get; set; }

		public AuthenticationException()
		{
		}

		public AuthenticationException(AuthenticationResultCode resultCode)
		{
			ResultCode = resultCode;
		}
	}
}

