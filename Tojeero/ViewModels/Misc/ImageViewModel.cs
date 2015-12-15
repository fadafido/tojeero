using System;
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

		public Func<Task<bool>> RemoveImageAction { get; set; }

		public Func<bool> CanPickImage { get; set; }

		public Action<IImageViewModel> DidPickImageAction { get; set; }

		public Func<Task<IImage>> PickImageFunction { get; set; }

		private string _imageID;

		public string ImageID
		{ 
			get
			{
				return _imageID; 
			}
			set
			{
				_imageID = value; 
				RaisePropertyChanged(() => ImageID); 
			}
		}

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

		private bool _isLoading;

		public bool IsLoading
		{ 
			get
			{
				return _isLoading; 
			}
			set
			{
				_isLoading = value; 
				RaisePropertyChanged(() => IsLoading); 
				RaisePropertyChanged(() => CanExecuteRemoveImageCommand);
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
				}, () => !IsLoading && CanPickImage != null ? CanPickImage() : true);
				return _pickImageCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _removeImageCommand;

		public System.Windows.Input.ICommand RemoveImageCommand
		{
			get
			{
				_removeImageCommand = _removeImageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
					await removeImage();
				}, () => CanExecuteRemoveImageCommand);
				return _removeImageCommand;
			}
		}

		public bool CanExecuteRemoveImageCommand
		{
			get
			{
				return !IsLoading && (this.NewImage != null || this.ImageUrl != null);
			}
		}

		#endregion

		#region Utility methods

		private async Task pickImage()
		{
			IsLoading = true;
			if (PickImageFunction != null)
			{
				var image = await PickImageFunction();
				if (image != null)
				{
					this.NewImage = image;
				}
			}	
			DidPickImageAction.Fire(this);
			IsLoading = false;
		}

		private async Task removeImage()
		{
			this.IsLoading = true;
			var shouldRemove = true;
			if(RemoveImageAction != null)
			{
				shouldRemove = await RemoveImageAction();
			}
			if(shouldRemove)
			{
				this.NewImage = null;
				this.ImageUrl = null;
			}
			this.IsLoading = false;
		}

		#endregion
	}
}

