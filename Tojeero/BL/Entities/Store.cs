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
				var newValue = value;
				if (value == null)
					newValue = Parse.ParseObject.Create<ParseStore>();
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
				if (_imageUrl == null && ImageUri != null)
					_imageUrl = ImageUri.ToString();
				return _imageUrl;
			}
			set
			{
				_imageUrl = value;
				ImageUri = value != null ? new Uri(value) : null;
			}
		}

		private Uri _imageUri;
		[Ignore]
		public Uri ImageUri
		{ 
			get
			{
				if (_imageUri == null && _parseObject != null && _parseObject.Image != null)
				{
					_imageUri = _parseObject.Image.Url;
				}
				return _imageUri; 
			}
			set
			{
				_imageUri = value; 
				RaisePropertyChanged(() => ImageUri); 
			}
		}

		#endregion
	}

	[ParseClassName("Store")]
	public class ParseStore : ParseObject
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

