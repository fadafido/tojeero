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
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class ImageControl : Grid
	{
		#region Constructors

		public ImageControl()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties

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

		private async Task<IImage> pickImage()
		{
			var parent = this.FindParent<Page>();
			var image = await ImageToolbox.PickImage(parent);
			return image;
		}

		#endregion
	}
}

