using System;
using Newtonsoft.Json;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
	public class User
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

		public string FullName
		{ 
			get
			{
				return string.Format("{0} {1}", FirstName, LastName); 
			}
		}

		public string UserName { get; set; }

		[JsonProperty("id")]
		public string ID { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		public string ProfilePictureUrl { get; set; }

		public string Country { get; set; }
		public string City { get; set; }
		public string Mobile { get; set; }
		public bool IsProfileSubmitted { get; set; }
		#endregion
	}
}

