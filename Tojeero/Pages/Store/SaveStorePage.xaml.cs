using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;
using System.Threading.Tasks;
using XLabs.Platform.Services.Media;
using System.IO;
using Cirrious.CrossCore;
using Tojeero.Core.Services;

namespace Tojeero.Forms
{
	public partial class SaveStorePage : ContentPage
	{
		#region Private fields

		private IMediaPicker _mediaPicker = Mvx.Resolve<IMediaPicker>();
		private IImageService _imageService = Mvx.Resolve<IImageService>(); 

		#endregion

		#region Constructors

		public SaveStorePage(IStore store)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<SaveStoreViewModel>();
			InitializeComponent();
			setupPickers();
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonCancel, "", async () =>
					{
						await this.Navigation.PopModalAsync();
					}));
			this.ViewModel.PickImageFunction = pickImage;
			TakePictureFunction = takePicture;
		}

		#endregion

		#region Properties

		private SaveStoreViewModel _viewModel;

		public SaveStoreViewModel ViewModel
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
				}
			}
		}

		public Func<bool, Task<IImage>> TakePictureFunction;

		#endregion

		#region View Lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ReloadCommand.Execute(null);
		}

		#endregion

		#region Utility methods

		private void setupPickers()
		{
			citiesPicker.Comparer = (c1, c2) =>
			{
				if (c1 == null || c2 == null)
					return false;
				return c1 == c2 || c1.ID == c2.ID;
			};
			countriesPicker.Comparer = (c1, c2) =>
			{
				if (c1 == null || c2 == null)
					return false;
				return c1 == c2 || c1.ID == c2.ID;
			};
			categoriesPicker.Comparer = (x, y) =>
			{
				if (x == null || y == null)
					return false;
				return x == y || x.ID == y.ID;
			};
		}


		async Task<IImage> pickImage()
		{
//			this.Navigation.PushModalAsync(new NavigationPage(new CameraPage()));
//			return null;
//
			IImage image = null;
			string[] titles = null;
			if (_mediaPicker.IsCameraAvailable)
			{
				titles = new string[] { AppResources.LabelFromCamera, AppResources.LabelFromLibrary };
			}
			if (titles != null)
			{
				var action = await this.DisplayActionSheet(AppResources.TitlePickImage, AppResources.ButtonCancel, null, titles);
				if (action == AppResources.LabelFromCamera)
				{
					image = await TakePictureFunction(true);
				}
				else if (action == AppResources.LabelFromLibrary)
				{
					image = await TakePictureFunction(false);
				}
			}
			else
			{
				image = await TakePictureFunction(false);
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
//					result = await this._mediaPicker.TakePhotoAsync(new CameraMediaStorageOptions
//						{ 
//							DefaultCamera = CameraDevice.Rear
//						});
					image = await this._imageService.GetImageFromCamera();
					return image;
				}
				else
				{
//					result = await this._mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions());
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

