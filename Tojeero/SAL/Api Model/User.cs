using System;

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

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }

		#endregion
	}
}

