using System;
using XLabs.Platform.Services.Media;
using System.IO;
using System.Threading.Tasks;
using Tojeero.Core.ViewModels;
using System.Drawing;


#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
using Tojeero.iOS;
#endif

#if __ANDROID__
using Android.Graphics;
using Tojeero.Droid;
#endif

namespace Tojeero.Core.Services
{
	public class ImageService : IImageService
	{
		public Task<PickedImage> GetImage(MediaFile file)
		{
			return Task<PickedImage>.Factory.StartNew(() =>
				{
					var rawImage = ResizeImage(file, 1000);

					//Initialize picked image
					PickedImage image = new PickedImage()
					{
						Name = System.IO.Path.GetFileName(file.Path),
						RawImage = rawImage
					};

					return image;
				});
		}

		public async Task<IImage> GetImageFromLibrary()
		{
			#if __IOS__

			#endif
			#if __ANDROID__
			var activity = Xamarin.Forms.Forms.Context as MainActivity;
			activity.StartActivity(typeof(PhotoGaleryActivity));
			#endif 

			return null;
		}

		public async Task<IImage> GetImageFromCamera()
		{
			#if __IOS__

			#endif
			#if __ANDROID__
			var activity = Xamarin.Forms.Forms.Context as MainActivity;
			activity.StartActivity(typeof(CameraActivity));
			#endif 

			return null;
		}

		public byte[] ResizeImage(MediaFile file, float maxPixelDimension)
		{
			#if __IOS__
			return ResizeImageIOS(file, maxPixelDimension);
			#endif
			#if __ANDROID__
			return ResizeImageAndroid ( file, maxPixelDimension );
			#endif 
		}


		#if __IOS__

		public byte[] ResizeImageIOS(MediaFile file, float maxPixelDimension)
		{
			byte[] rawImage = new byte[file.Source.Length];
			file.Source.Read(rawImage, 0, rawImage.Length);

			using (var original = rawImage.GetUIImage())
			using(var result = original.GetScaledAndRotatedImage(maxPixelDimension))
			{
				var fileName = file.Path.ToLower();
				if (fileName.EndsWith("png"))
				{
					return result.GetRawBytes(ImageType.Png);
				}
				return result.GetRawBytes(ImageType.Jpeg);
			}
		}

		#endif

		#if __ANDROID__
		
		public byte[] ResizeImageAndroid (MediaFile file, float maxPixelDimension)
		{
			byte[] rawImage = new byte[file.Source.Length];
			file.Source.Read(rawImage, 0, rawImage.Length);

			using (var original = rawImage.GetBitmap())
			using(var result = original.GetScaledAndRotatedBitmap(file.Exif.Orientation, maxPixelDimension))
			{
				var format = Bitmap.CompressFormat.Jpeg;
				var fileName = file.Path.ToLower();
				if (fileName.EndsWith("png"))
				{
					format = Bitmap.CompressFormat.Png;
				}
				return result.GetRawBytes(format);
			}
		}
			
		#endif
	}
}

