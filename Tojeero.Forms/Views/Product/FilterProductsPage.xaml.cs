using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Product
{
	public partial class FilterProductsPage : ContentPage
	{
		#region Constructors

		public FilterProductsPage(string query = null)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<FilterProductsViewModel>();
			this.ViewModel.Query = query;
			InitializeComponent();
			setupPickers();
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, "", async () =>
				{
					await this.Navigation.PopModalAsync();
					this.ViewModel.DoneCommand.Execute(null);
				}));
		}

		#endregion

		#region Properties

		private FilterProductsViewModel _viewModel;

		public FilterProductsViewModel ViewModel
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
					this.BindingContext = _viewModel;
				}
			}
		}

		#endregion

		#region View Lifecycle

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ReloadCommand.Execute(null);
			await this.ViewModel.ReloadCount();
		}

		#endregion

		#region Utility methods

		private void setupPickers()
		{
			countriesPicker.FacetsLoader = this.ViewModel.FetchCountryFacets;
			countriesPicker.ObjectsLoader = () => Task<IList<ICountry>>.Factory.StartNew(() => this.ViewModel.Countries);

			citiesPicker.FacetsLoader = this.ViewModel.FetchCityFacets;
			citiesPicker.ObjectsLoader = () => Task<IList<ICity>>.Factory.StartNew(() => this.ViewModel.Cities);

			categoriesPicker.FacetsLoader = this.ViewModel.FetchCategoryFacets;
			categoriesPicker.ObjectsLoader = () => Task<IList<IProductCategory>>.Factory.StartNew(() => this.ViewModel.Categories);

			subcategoriesPicker.FacetsLoader = this.ViewModel.FetchSubcategoryFacets;
			subcategoriesPicker.ObjectsLoader = () => Task<IList<IProductSubcategory>>.Factory.StartNew(() => this.ViewModel.Subcategories);
		}
			
		#endregion
	}
}

