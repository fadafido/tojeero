using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Views.Main;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Store
{
    public partial class SaveStorePage
    {
        #region Private fields and properties

        private bool _toolbarButtonsAdded;
        private readonly IStore _store;

        #endregion

        #region Constructors

        public SaveStorePage(IStore store)
        {
            InitializeComponent();

            _store = store;
            ViewModel = MvxToolbox.LoadViewModel<SaveStoreViewModel>();
            
            SetupPickers();
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
            ViewModel.CurrentStore = _store;
            mainImageControl.ViewModel = ViewModel.MainImage;
        }

        #endregion

        #region View Lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_toolbarButtonsAdded)
            {
                //if this view is not inside root page add close button
                var root = this.FindParent<RootPage>();
                if (root == null)
                {
                    ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "",
                        async () => { await Navigation.PopModalAsync(); }));
                }


                ViewModel.DidSaveStoreAction = async (savedStore, isNew) =>
                {
                    if (isNew)
                    {
                        //If the view isn't inside tabs, pop it
                        if (root == null)
                        {
                            await Navigation.PopModalAsync();

                            var storeInfoPage = new StoreInfoPage(savedStore, ContentMode.Edit);
                            await FormsApp.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(storeInfoPage));
                        }
                    }
                    else
                    {
                        //If the view isn't inside tabs, pop it
                        if (root == null)
                            await Navigation.PopAsync();
                    }
                };
                _toolbarButtonsAdded = true;
            }
        }

        #endregion

        #region Utility methods

        private void SetupPickers()
        {
            categoriesPicker.ItemsLoader =
                () => Task<IList<IStoreCategory>>.Factory.StartNew(() => ViewModel.Categories);
            categoriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;

            countriesPicker.ItemsLoader = () => Task<IList<ICountry>>.Factory.StartNew(() => ViewModel.Countries);
            countriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;

            citiesPicker.ItemsLoader = () => Task<IList<ICity>>.Factory.StartNew(() => ViewModel.Cities);
            citiesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;
        }

        #endregion
    }
}