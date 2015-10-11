using System;
using Parse;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public class SearchableParseObject : ParseObject
	{
		#region Constructors

		public SearchableParseObject()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("searchTokens")]
		public IList<string> SearchTokens
		{
			get
			{
				return GetProperty<IList<string>>();
			}
			set
			{
				SetProperty<IList<string>>(value);
			}
		}

		#endregion
	}
}

