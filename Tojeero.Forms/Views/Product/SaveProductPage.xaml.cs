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
    public partial class SaveProductPage
    {
        #region Private fields

        private readonly IProduct _currentProduct;
        private readonly IStore _parentStore;

        #endregion


        #region Constructors

        public SaveProductPage(IProduct product, IStore parentStore)
        {
            InitializeComponent();

            _parentStore = parentStore;
            _currentProduct = product;

            ViewModel = MvxToolbox.LoadViewModel<SaveProductViewModel>();

            SetupPickers();
            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "",
                async () => { await Navigation.PopModalAsync(); }));
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
            ViewModel.CurrentProduct = _currentProduct;
            ViewModel.Store = _parentStore;


            ViewModel.CurrentProduct = _currentProduct;

            mainImageControl.ViewModel = ViewModel.MainImage;

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

        #region Utility methods

        private void SetupPickers()
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