using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;
using Tojeero.Core;
using System.Threading.Tasks;

namespace Tojeero.Forms
{
	public partial class FilterStoresPage : ContentPage
	{
		#region Constructors

		public FilterStoresPage()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<FilterStoresViewModel>();
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

		private FilterStoresViewModel _viewModel;

		public FilterStoresViewModel ViewModel
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

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ReloadCommand.Execute(null);
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
			categoriesPicker.ObjectsLoader = () => Task<IList<IStoreCategory>>.Factory.StartNew(() => this.ViewModel.Categories);
		}

		#endregion
	}
}

