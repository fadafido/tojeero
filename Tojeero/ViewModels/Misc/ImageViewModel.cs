using System;
using Cirrious.MvvmCross.ViewModels;
using Xamarin.Forms;
using System.IO;

namespace Tojeero.Core.ViewModels
{
	public class ImageViewModel : MvxViewModel, IImageViewModel
	{
		public ImageSource Image
		{
			get
			{
				if (NewImage != null && NewImage.RawImage != null)
				{
					Stream stream = new MemoryStream(NewImage.RawImage);
					var source = ImageSource.FromStream(() => stream);
					return source;
				}
				else if (!string.IsNullOrEmpty(ImageUrl))
				{
					var source = new UriImageSource()
						{
							Uri = new Uri(ImageUrl),
							CachingEnabled = true,
							CacheValidity = Constants.ImageCacheTimespan
						};
					return source;
				}
				return null;
			}
		}

		
		private string _imageUrl;

		public string ImageUrl
		{ 
			get
			{
				return _imageUrl; 
			}
			set
			{
				_imageUrl = value; 
				RaisePropertyChanged(() => ImageUrl); 
				RaisePropertyChanged(() => Image);
			}
		}

		private IImage _newImage;

		public IImage NewImage
		{ 
			get
			{
				return _newImage; 
			}
			set
			{
				_newImage = value; 
				RaisePropertyChanged(() => NewImage); 
				RaisePropertyChanged(() => Image);
			}
		}
	}
}

