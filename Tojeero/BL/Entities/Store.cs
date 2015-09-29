using System;
using Parse;

namespace Tojeero.Core
{
	[ParseClassName("Store")]
	public class Store : BaseModelEntity, IStore
	{
		#region Constructors

		public Store()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("name")]
		public string Name
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
			}
		}

		public Uri ImageUrl
		{
			get
			{
				return Image != null ? Image.Url : null;
			}
		}

		[ParseFieldName("image")]
		public ParseFile Image
		{
			get
			{
				return GetProperty<ParseFile>();
			}
			set
			{
				SetProperty<ParseFile>(value);
			}
		}

		#endregion
	}
}

