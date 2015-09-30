using System;
using Xamarin.Forms;
using Tojeero.Core;

namespace Tojeero.Forms
{
	public class UrlToCachedImageSourceConverter : IValueConverter
	{

		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return null;
			var url = value.ToString();
			try
			{
				var source = new UriImageSource()
					{
						Uri = new Uri(url),
						CachingEnabled = true,
						CacheValidity = Constants.ImageCacheTimespan
					};
				return source;
			}
			catch
			{
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

