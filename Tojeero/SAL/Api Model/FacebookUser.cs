using System;
using Newtonsoft.Json;

namespace Tojeero.Core
{
	public class FacebookUser
	{
		public FacebookUser()
		{
		}
			
		public string Email { get; set; }
		public string ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Token { get; set; }
		public string ProfilePictureUri { get; set; }
		public DateTime ExpiryDate { get; set; }
	}
}

