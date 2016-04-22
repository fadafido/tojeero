using System;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
	public class FacebookUser
	{
		public FacebookUser()
		{
		}
			
		public IUser User { get; set; }
		public string Token { get; set; }
		public DateTime ExpiryDate { get; set; }
	}
}

