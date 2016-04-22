using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.ViewModels.Contracts;
using Tojeero.Forms.Toolbox;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
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

