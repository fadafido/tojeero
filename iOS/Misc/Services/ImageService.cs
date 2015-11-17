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
using Cirrious.CrossCore;

namespace Tojeero.Core.Services
{
	public class ImageService : IImageService
	{
		#region Constructors

		public ImageService(IMvxMessenger messenger)
		{
			
		}

		#endregion

		#region IImageService implementation

		public async Task<IImage> GetImageFromLibrary()
		{
			IMediaPicker _mediaPicker = Mvx.Resolve<IMediaPicker>();
			var imageFile = await _mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions());
			var result = await getImageFromMediaFile(imageFile);
			return result;
		}

		public async Task<IImage> GetImageFromCamera()
		{
			IMediaPicker _mediaPicker = Mvx.Resolve<IMediaPicker>();
			var imageFile = await _mediaPicker.TakePhotoAsync(new CameraMediaStorageOptions
				{ 
					DefaultCamera = CameraDevice.Rear
				});
			var result = await getImageFromMediaFile(imageFile);
			return result;
		}

		#endregion
			

		#region Utility methods

		public Task<IImage> getImageFromMediaFile(MediaFile file)
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

		public byte[] getScaledAndRotatedImage(MediaFile file, float maxPixelDimension)
		{
			byte[] rawImage = new byte[file.Source.Length];
			file.Source.Read(rawImage, 0, rawImage.Length);

			using (var original = rawImage.GetUIImage())
			using (var result = original.GetScaledAndRotatedImage(maxPixelDimension))
			{
				var fileName = file.Path.ToLower();
				if (fileName.EndsWith("png"))
				{
					return result.GetRawBytes(ImageType.Png);
				}
				return result.GetRawBytes(ImageType.Jpeg);
			}
		}
			
		#endregion
	}
}

