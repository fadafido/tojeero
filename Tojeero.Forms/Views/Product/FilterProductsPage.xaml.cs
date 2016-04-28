﻿using System.Collections.Generic;
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
            ViewModel = MvxToolbox.LoadViewModel<FilterProductsViewModel>();
            ViewModel.Query = query;
            InitializeComponent();
            setupPickers();
            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, "", async () =>
            {
                await Navigation.PopModalAsync();
                ViewModel.DoneCommand.Execute(null);
            }));
        }

        #endregion

        #region Properties

        private FilterProductsViewModel _viewModel;

        public FilterProductsViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    BindingContext = _viewModel;
                }
            }
        }

        #endregion

        #region View Lifecycle

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.ReloadCommand.Execute(null);
            await ViewModel.ReloadCount();
        }

        #endregion

        #region Utility methods

        private void setupPickers()
        {
            countriesPicker.FacetsLoader = ViewModel.FetchCountryFacets;
            countriesPicker.ObjectsLoader = () => Task<IList<ICountry>>.Factory.StartNew(() => ViewModel.Countries);

            citiesPicker.FacetsLoader = ViewModel.FetchCityFacets;
            citiesPicker.ObjectsLoader = () => Task<IList<ICity>>.Factory.StartNew(() => ViewModel.Cities);

            categoriesPicker.FacetsLoader = ViewModel.FetchCategoryFacets;
            categoriesPicker.ObjectsLoader =
                () => Task<IList<IProductCategory>>.Factory.StartNew(() => ViewModel.Categories);

            subcategoriesPicker.FacetsLoader = ViewModel.FetchSubcategoryFacets;
            subcategoriesPicker.ObjectsLoader =
                () => Task<IList<IProductSubcategory>>.Factory.StartNew(() => ViewModel.Subcategories);
        }

        #endregion
    }
}