﻿using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core;
using Tojeero.Core.Logging;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.ViewModels.Image;
using Tojeero.Droid.Images;
using Tojeero.Droid.Messages;
using Tojeero.Droid.Toolbox;
using XLabs.Platform.Services.Media;

namespace Tojeero.Droid.Services
{
	public class ImageService : IImageService
	{
		#region Private fields and properties

		private MvxSubscriptionToken _tokenCamera;
		private MvxSubscriptionToken _tokenLibrary;

		private TaskCompletionSource<IImage> _selectImageCompletion;
		private Guid _receiverID = Guid.NewGuid();

		#endregion

		#region Constructors

		public ImageService(IMvxMessenger messenger)
		{
			_tokenCamera = messenger.Subscribe<CameraImageSelectedMessage>(cameraImageSelected);
			_tokenLibrary = messenger.Subscribe<LibraryImageSelectedMessage>(libraryImageSelected);
		}

		#endregion

	
		#region IImageService

		public Task<IImage> GetImageFromLibrary()
		{
			_selectImageCompletion = new TaskCompletionSource<IImage>();
			var activity = Xamarin.Forms.Forms.Context as MainActivity;
			var intent = PhotoGaleryActivity.GetIntentForReceiver(activity, _receiverID);
			activity.StartActivity(intent);
			return _selectImageCompletion.Task;
		}

		public Task<IImage> GetImageFromCamera()
		{
			_selectImageCompletion = new TaskCompletionSource<IImage>();
			var activity = Xamarin.Forms.Forms.Context as MainActivity;
			var intent = CameraActivity.GetIntentForReceiver(activity, _receiverID);
			activity.StartActivity(intent);
			return _selectImageCompletion.Task;
		}

		#endregion

		#region Messages

		private async void cameraImageSelected(CameraImageSelectedMessage message)
		{
			if (message.ReceiverID == _receiverID)
			{
				IImage result = await Task<IImage>.Factory.StartNew(() =>
					{
						IImage image = null;
						if (message.Image != null)
						{
							image = new PickedImage()
							{
								Name = Guid.NewGuid().ToString() + ".jpg",
								RawImage = message.Image.GetRawBytes(Bitmap.CompressFormat.Jpeg)
							};
						}
						return image;
					});

				_selectImageCompletion.TrySetResult(result);
			}
		}

		private async void libraryImageSelected(LibraryImageSelectedMessage message)
		{
			if (message.ReceiverID == _receiverID)
			{
				IImage result = await Task<IImage>.Factory.StartNew(() =>
					{
						IImage image = null;
						try
						{
							if (message.Path != null)
							{
								var path = message.Path;
								var file = new MediaFile(path, () => File.OpenRead(path));
								var rawImage = getScaledAndRotatedImage(file, Constants.MaxPixelDimensionOfImages);

								//Initialize picked image
								image = new PickedImage()
									{
										Name = System.IO.Path.GetFileName(path),
										RawImage = rawImage
									};
							}	
						}
						catch (Exception ex)
						{
							Tools.Logger.Log(ex, LoggingLevel.Error, true);	
						}

						return image;
					});

				_selectImageCompletion.TrySetResult(result);
			}
		}

		#endregion

		#region Utility methods

		private byte[] getScaledAndRotatedImage(MediaFile file, float maxPixelDimension)
		{
			byte[] rawImage = new byte[file.Source.Length];
			file.Source.Read(rawImage, 0, rawImage.Length);

			using (var original = rawImage.GetBitmap())
			using (var result = original.GetScaledAndRotatedBitmap(file.Exif.Orientation, maxPixelDimension))
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

		#endregion
	}
}

