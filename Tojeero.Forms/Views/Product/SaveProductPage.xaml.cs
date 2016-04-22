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
			this.ViewModel = MvxToolbox.LoadViewModel<SaveProductViewModel>();
			this.ViewModel.CurrentProduct = product;
			this.ViewModel.Store = parentStore;

			InitializeComponent();
			setupPickers();
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "", async () =>
					{
						await this.Navigation.PopModalAsync();
					}));
			this.ViewModel.CurrentProduct = product;

			this.mainImageControl.ViewModel = this.ViewModel.MainImage;
			this.ViewModel.ShowAlert = (t, m, accept) =>
			{
				this.DisplayAlert(t, m, accept);
			};
			
			this.ViewModel.DidSaveProductAction = async (savedProduct, isNew) =>
			{
				if (isNew)
				{
					await this.Navigation.PopModalAsync();
					var productInfoPage = new ProductDetailsPage(savedProduct, ContentMode.Edit);
					productInfoPage.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "", async () =>
							{
								await productInfoPage.Navigation.PopModalAsync();
							}));
					FormsApp.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(productInfoPage));
				}
				else
				{
					await this.Navigation.PopAsync();
				}
			};

			this.multiImagePicker.ImageFactory = this.ViewModel.ImageViewModelFactory;
		}

		#endregion

		#region Properties

		private SaveProductViewModel _viewModel;

		public SaveProductViewModel ViewModel
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
            categoriesPicker.ItemsLoader = () => Task<IList<IProductCategory>>.Factory.StartNew(() => this.ViewModel.Categories );
		    categoriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;

            subcategoriesPicker.ItemsLoader = () => Task<IList<IProductSubcategory>>.Factory.StartNew(() => this.ViewModel.Subcategories);
            subcategoriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;
        }

		#endregion
	}
}

