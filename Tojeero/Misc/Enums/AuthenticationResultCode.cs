using System;
using System.Net;

namespace Tojeero.Core
{
	public enum AuthenticationResultCode
	{
		Unknown,
		WrongCredentials,
		Successful,
		DuplicateUser,
		TokenExpired,
		TokenNotFound,
		WebException,
		Unauthorized
	}
	
}
