using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parse;
using System.Linq;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core
{
	public class UserManager : IUserManager
	{
		#region IUserManager implementation

		public IUser Create()
		{
			return new User();
		}

		#endregion
	}


}

