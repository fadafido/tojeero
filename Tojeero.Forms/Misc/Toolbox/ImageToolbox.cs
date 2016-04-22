using System;
using System.Threading.Tasks;
using Tojeero.Core;
using XLabs.Platform.Services.Media;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Tojeero.Core.Logging;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Xamarin.Forms;

namespace Tojeero.Forms.Toolbox
{
	public static class ImageToolbox
	{
		#region Private fields

		private static IMediaPicker _mediaPicker = Mvx.Resolve<IMediaPicker>();
		private static IImageService _imageService = Mvx.Resolve<IImageService>(); 

		#endregion

		#region Static API

		public static async Task<IImage> PickImage(Page parent)
		{
			IImage image = null;
			string[] titles = null;
			if (_mediaPicker.IsCameraAvailable)
			{
				titles = new string[] { AppResources.LabelFromCamera, AppResources.LabelFromLibrary };
			}
			if (titles != null)
			{				
				if (parent != null)
				{
					var action = await parent.DisplayActionSheet(AppResources.TitlePickImage, AppResources.ButtonCancel, null, titles);
					if (action == AppResources.LabelFromCamera)
					{
						image = await takePicture(true);
					}
					else if (action == AppResources.LabelFromLibrary)
					{
						image = await takePicture(false);
					}
				}
			}
			else
			{
				image = await takePicture(false);
			}
			return image;
		}

		#endregion

		#region Utility methods

		private static async Task<IImage> takePicture(bool fromCamera)
		{
			try
			{
				MediaFile result = null;
				IImage image;
				if (fromCamera)
				{
					image = await _imageService.GetImageFromCamera();
					return image;
				}
				else
				{
					image = await _imageService.GetImageFromLibrary();
					return image;
				}
				return image;
			}
			catch (Exception ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning, true);
			}
			return null;
		}

		#endregion
	}
}

