using System;
using Newtonsoft.Json;

namespace Tojeero.Core
{
	public class FacebookUser
	{
		public FacebookUser()
		{
		}
			
		public User User { get; set; }
		public string Token { get; set; }
		public DateTime ExpiryDate { get; set; }
	}
}

