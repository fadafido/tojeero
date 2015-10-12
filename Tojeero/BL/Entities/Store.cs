using System;
using Parse;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;

namespace Tojeero.Core
{
	public class Store : BaseModelEntity<ParseStore>, IStore
	{
		#region Constructors

		public Store()
			:base()
		{
			
		}

		public Store(ParseStore parseStore = null)
			: base(parseStore)
		{

		}

		#endregion

		#region Properties

		[Ignore]
		public override ParseStore ParseObject
		{
			get
			{
				return base.ParseObject;
			}
			set
			{
				_imageUrl = null;
				base.ParseObject = value;
			}
		}

		public string Name
		{
			get
			{
				return _parseObject.Name;
			}
			set
			{
				_parseObject.Name = value;
				RaisePropertyChanged(() => Name);
			}
		}

		private string _imageUrl;
		public string ImageUrl
		{
			get
			{
				if (_imageUrl == null && _parseObject != null && _parseObject.Image != null && _parseObject.Image.Url != null)
					_imageUrl = _parseObject.Image.Url.ToString();
				return _imageUrl;
			}
			set
			{
				_imageUrl = value;
				RaisePropertyChanged(() => ImageUrl);
			}
		}

		private string _categoryID;
		public string CategoryID
		{ 
			get
			{
				if (_categoryID == null && _parseObject != null && _parseObject.Category != null)
					_categoryID = _parseObject.Category.ObjectId;
				return _categoryID; 
			}
			set
			{
				_categoryID = value; 
			}
		}
		#endregion
	}

	[ParseClassName("Store")]
	public class ParseStore : SearchableParseObject
	{
		#region Constructors

		public ParseStore()
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

		[ParseFieldName("description")]
		public string Description
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

		[ParseFieldName("category")]
		public ParseStoreCategory Category
		{
			get
			{
				return GetProperty<ParseStoreCategory>();
			}
			set
			{
				SetProperty<ParseStoreCategory>(value);
			}
		}
		#endregion
	}
}

