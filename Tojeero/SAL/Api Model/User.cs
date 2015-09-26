using System;
using Newtonsoft.Json;

namespace Tojeero.Core
{
	public class User : BaseModelEntity
	{
		#region Constructors

		public User()
			: base()
		{
		}

		#endregion

		#region Properties

		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		[JsonProperty("last_name")]
		public string LastName { get; set; }

		public string UserName { get; set; }

		[JsonProperty("id")]
		public string ID { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		public string ProfilePictureUrl { get; set; }
		#endregion
	}
}

