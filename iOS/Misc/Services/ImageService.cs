using System;
using XLabs.Platform.Services.Media;
using System.IO;
using System.Threading.Tasks;
using Tojeero.Core.ViewModels;
using System.Drawing;
using Cirrious.MvvmCross.Plugins.Messenger;

using System.Drawing;
using UIKit;
using CoreGraphics;
using Tojeero.iOS;

namespace Tojeero.Core.Services
{
	public class ImageService : IImageService
	{
		public ImageService(IMvxMessenger messenger)
		{
			
		}

		public Task<IImage> GetImage(MediaFile file)
		{
			return Task<IImage>.Factory.StartNew(() =>
				{
					var rawImage = getScaledAndRotatedImage(file, 1000);

					//Initialize picked image
					PickedImage image = new PickedImage()
					{
						Name = System.IO.Path.GetFileName(file.Path),
						RawImage = rawImage
					};

					return image;
				});
		}

		public Task<IImage> GetImageFromLibrary()
		{
			return null;
		}

		public Task<IImage> GetImageFromCamera()
		{
			
			return null;
		}
			
		public byte[] getScaledAndRotatedImage(MediaFile file, float maxPixelDimension)
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
	}
}

