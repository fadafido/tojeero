using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;
using System.Threading.Tasks;
using XLabs.Platform.Services.Media;
using System.IO;
using Cirrious.CrossCore;
using Tojeero.Core.Services;

namespace Tojeero.Forms
{
	public partial class SaveStorePage : ContentPage
	{

		#region Constructors

		public SaveStorePage(IStore store)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<SaveStoreViewModel>();
			InitializeComponent();
			setupPickers();
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "", async () =>
					{
						await this.Navigation.PopModalAsync();
					}));
			this.ViewModel.CurrentStore = store;
			this.mainImageControl.ViewModel = this.ViewModel.MainImage;
			this.ViewModel.ShowAlert = (t, m, accept) =>
			{
				this.DisplayAlert(t, m, accept);
			};
			
			this.ViewModel.DidSaveStoreAction = async (savedStore, isNew) => {
				if(isNew)
				{
					await this.Navigation.PopModalAsync();
					var storeInfoPage = new StoreInfoPage(savedStore, ContentMode.Edit);
					storeInfoPage.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "", async () =>
						{
							await storeInfoPage.Navigation.PopModalAsync();
						}));
					FormsApp.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(storeInfoPage));
				}
				else
				{
					await this.Navigation.PopAsync();
				}
			};
		}

		#endregion

		#region Properties

		private SaveStoreViewModel _viewModel;

		public SaveStoreViewModel ViewModel
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
			citiesPicker.Comparer = (c1, c2) =>
			{
				if (c1 == null || c2 == null)
					return false;
				return c1 == c2 || c1.ID == c2.ID;
			};
			countriesPicker.Comparer = (c1, c2) =>
			{
				if (c1 == null || c2 == null)
					return false;
				return c1 == c2 || c1.ID == c2.ID;
			};
			categoriesPicker.Comparer = (x, y) =>
			{
				if (x == null || y == null)
					return false;
				return x == y || x.ID == y.ID;
			};
		}

		#endregion
	}
}

