using System;
using System.Net;

namespace Tojeero.Core
{

	public class AuthenticationResult
	{
		public System.Net.HttpStatusCode HttpStatusCode { get; set; }
		public AuthenticationResultCode ResultCode { get; set; }
		public Exception Error { get; set; }
		public AuthenticationResult (AuthenticationResultCode resultCode, Exception error = null)
		{
			ResultCode = resultCode;
			Error = error;
		}

		public AuthenticationResult (AuthenticationResultCode resultCode, HttpStatusCode httpStatusCode, Exception error = null)
		{
			ResultCode = resultCode;
			Error = error;
			HttpStatusCode = httpStatusCode;
		}

		public string ErrorMessage{ get; set;}
	}

	public class AuthenticationResult<T> : AuthenticationResult
	{
		public T Data { get; set; }
		public AuthenticationResult (AuthenticationResultCode resultCode, Exception error = null) : base(resultCode, error)
		{
			Data = default(T);
		}

		public AuthenticationResult(T data, AuthenticationResultCode resultCode, Exception error = null) : base(resultCode, error)
		{
			Data = data;
		}

		public AuthenticationResult (T data, AuthenticationResultCode resultCode, HttpStatusCode httpStatusCode, Exception error = null) : base(resultCode, httpStatusCode, error)
		{
			Data = data;
		}
	}
}

