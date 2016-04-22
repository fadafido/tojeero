using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Product
{
    public partial class SaveProductPage : ContentPage
    {
        #region Constructors

        public SaveProductPage(IProduct product, IStore parentStore)
        {
            ViewModel = MvxToolbox.LoadViewModel<SaveProductViewModel>();
            ViewModel.CurrentProduct = product;
            ViewModel.Store = parentStore;

            InitializeComponent();
            setupPickers();
            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "",
                async () => { await Navigation.PopModalAsync(); }));
            ViewModel.CurrentProduct = product;

            mainImageControl.ViewModel = ViewModel.MainImage;
            ViewModel.ShowAlert = (t, m, accept) => { DisplayAlert(t, m, accept); };

            ViewModel.DidSaveProductAction = async (savedProduct, isNew) =>
            {
                if (isNew)
                {
                    await Navigation.PopModalAsync();
                    var productInfoPage = new ProductDetailsPage(savedProduct, ContentMode.Edit);
                    productInfoPage.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "",
                        async () => { await productInfoPage.Navigation.PopModalAsync(); }));
                    FormsApp.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(productInfoPage));
                }
                else
                {
                    await Navigation.PopAsync();
                }
            };

            multiImagePicker.ImageFactory = ViewModel.ImageViewModelFactory;
        }

        #endregion

        #region Properties

        private SaveProductViewModel _viewModel;

        public SaveProductViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    BindingContext = value;
                }
            }
        }

        #endregion

        #region View Lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.ReloadCommand.Execute(null);
        }

        #endregion

        #region Utility methods

        private void setupPickers()
        {
            categoriesPicker.ItemsLoader =
                () => Task<IList<IProductCategory>>.Factory.StartNew(() => ViewModel.Categories);
            categoriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;

            subcategoriesPicker.ItemsLoader =
                () => Task<IList<IProductSubcategory>>.Factory.StartNew(() => ViewModel.Subcategories);
            subcategoriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;
        }

        #endregion
    }
}