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
		public string[] SearchTokens
		{
			get
			{
				return GetProperty<string[]>();
			}
			set
			{
				SetProperty<string[]>(value);
			}
		}

		#endregion
	}
}

