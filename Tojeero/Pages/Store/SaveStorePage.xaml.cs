﻿using System;
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

namespace Tojeero.Forms
{
	public partial class SaveStorePage : ContentPage
	{
		#region Private fields

		private IMediaPicker _mediaPicker = Mvx.Resolve<IMediaPicker>();

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
			PickedImage image = null;
			try
			{
				MediaFile result = null;
				if (fromCamera)
				{
					result = await this._mediaPicker.TakePhotoAsync(new CameraMediaStorageOptions
						{ 
							DefaultCamera = CameraDevice.Rear, 
							MaxPixelDimension = 1000
						});
				}
				else
				{
					result = await this._mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
						{
							MaxPixelDimension = 1000
						});
				}
				if (result != null)
				{
					byte[] rawImage = new byte[result.Source.Length];
					result.Source.Read(rawImage, 0, rawImage.Length);
					return new PickedImage()
					{
						Name = Path.GetFileName(result.Path),
						RawImage = rawImage
					};
				}
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

