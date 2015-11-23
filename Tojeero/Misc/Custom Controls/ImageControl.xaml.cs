using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using Tojeero.Core;
using XLabs.Platform.Services.Media;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Tojeero.Forms.Resources;
using Tojeero.Core.ViewModels;

namespace Tojeero.Forms
{
	public partial class ImageControl : Grid
	{
		#region Private fields

		private IMediaPicker _mediaPicker = Mvx.Resolve<IMediaPicker>();
		private IImageService _imageService = Mvx.Resolve<IImageService>(); 

		#endregion

		#region Constructors

		public ImageControl()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties

		public Page ParentPage { get; set; }

		private IImageViewModel _viewModel;

		public IImageViewModel ViewModel
		{
			get
			{
				return _viewModel;
			}
			set
			{
				if (_viewModel != value)
				{
					_viewModel = value;
					this.BindingContext = value;
					if (_viewModel != null)
					{
						_viewModel.PickImageFunction = pickImage;
					}
				}
			}
		}

		#endregion

		#region Utility methods

		async Task<IImage> pickImage()
		{
			IImage image = null;
			string[] titles = null;
			if (_mediaPicker.IsCameraAvailable)
			{
				titles = new string[] { AppResources.LabelFromCamera, AppResources.LabelFromLibrary };
			}
			if (titles != null)
			{
				var action = await this.ParentPage.DisplayActionSheet(AppResources.TitlePickImage, AppResources.ButtonCancel, null, titles);
				if (action == AppResources.LabelFromCamera)
				{
					image = await takePicture(true);
				}
				else if (action == AppResources.LabelFromLibrary)
				{
					image = await takePicture(false);
				}
			}
			else
			{
				image = await takePicture(false);
			}
			return image;
		}

		private async Task<IImage> takePicture(bool fromCamera)
		{
			try
			{
				MediaFile result = null;
				IImage image;
				if (fromCamera)
				{
					image = await this._imageService.GetImageFromCamera();
					return image;
				}
				else
				{
					image = await this._imageService.GetImageFromLibrary();
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

