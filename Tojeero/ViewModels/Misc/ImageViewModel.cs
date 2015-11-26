﻿using System;
using Cirrious.MvvmCross.ViewModels;
using Xamarin.Forms;
using System.IO;
using System.Threading.Tasks;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels
{
	public class ImageViewModel : MvxViewModel, IImageViewModel
	{
		#region Properties

		public Action<IImageViewModel> RemoveImageAction { get; set; }

		public Action<IImageViewModel> DidPickImageAction { get; set; }

		public Func<Task<IImage>> PickImageFunction { get; set; }

		ImageSource _image;
		public ImageSource Image
		{
			get
			{
				if (_image != null)
					return _image;
				if (NewImage != null && NewImage.RawImage != null)
				{
					Stream stream = new MemoryStream(NewImage.RawImage);
					_image = ImageSource.FromStream(() => stream);
				}
				else if (!string.IsNullOrEmpty(ImageUrl))
				{
					_image = new UriImageSource()
					{
						Uri = new Uri(ImageUrl),
						CachingEnabled = true,
						CacheValidity = Constants.ImageCacheTimespan
					};
				}
				return _image;
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
				if (_imageUrl != value)
				{
					_imageUrl = value; 
					_image = null;
					RaisePropertyChanged(() => ImageUrl); 
					RaisePropertyChanged(() => Image);
					RaisePropertyChanged(() => CanExecuteRemoveImageCommand);
				}
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
				if (_newImage != value)
				{
					_newImage = value; 
					_image = null;
					RaisePropertyChanged(() => NewImage); 
					RaisePropertyChanged(() => Image);
					RaisePropertyChanged(() => CanExecuteRemoveImageCommand);
				}
			}
		}

		private bool _isPickingImage;

		public bool IsPickingImage
		{ 
			get
			{
				return _isPickingImage; 
			}
			set
			{
				_isPickingImage = value; 
				RaisePropertyChanged(() => IsPickingImage); 
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _pickImageCommand;

		public System.Windows.Input.ICommand PickImageCommand
		{
			get
			{
				_pickImageCommand = _pickImageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
					await pickImage();
				}, () => !IsPickingImage);
				return _pickImageCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _removeImageCommand;

		public System.Windows.Input.ICommand RemoveImageCommand
		{
			get
			{
				_removeImageCommand = _removeImageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					this.NewImage = null;
					this.ImageUrl = null;
					this.RemoveImageAction.Fire(this);
				}, () => CanExecuteRemoveImageCommand);
				return _removeImageCommand;
			}
		}

		public bool CanExecuteRemoveImageCommand
		{
			get
			{
				return this.NewImage != null || this.ImageUrl != null;
			}
		}

		#endregion

		#region Utility methods

		private async Task pickImage()
		{
			IsPickingImage = true;
			if (PickImageFunction != null)
			{
				var image = await PickImageFunction();
				if (image != null)
				{
					this.NewImage = image;
				}
			}	
			DidPickImageAction.Fire(this);
			IsPickingImage = false;
		}

		#endregion
	}
}

