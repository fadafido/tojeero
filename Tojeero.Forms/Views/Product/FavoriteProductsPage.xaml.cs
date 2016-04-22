using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;
using ListView = Tojeero.Forms.Controls.ListView;

namespace Tojeero.Forms.Views.Product
{
	public partial class FavoriteProductsPage : BaseCollectionPage
	{
		#region Constructors

		public FavoriteProductsPage()
			: base()
		{
			InitializeComponent();
			this.ViewModel = MvxToolbox.LoadViewModel<FavoriteProductsViewModel>();
			ListView.ItemSelected += itemSelected;
		}

		#endregion

		#region Properties

		public new FavoriteProductsViewModel ViewModel
		{
			get
			{
				return base.ViewModel as FavoriteProductsViewModel;
			}
			set
			{
				base.ViewModel = value;
			}
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.LoadFirstPageCommand.Execute(null);
		}

		#endregion

		#region UI Events

		private async void itemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = ((ListView)sender).SelectedItem as ProductViewModel; 
			if (item != null)
			{
				((ListView)sender).SelectedItem = null;
				var productDetails = new ProductDetailsPage(item.Product);
				await this.Navigation.PushAsync(productDetails);
			}
		}

		#endregion
	}
}

