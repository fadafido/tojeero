using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Core;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class StoreDetailsPage : ContentPage
	{
		
		#region Constructors

		public StoreDetailsPage(IStore store, ContentMode mode = ContentMode.View)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<StoreDetailsViewModel>();
			this.ViewModel.Store = store;
			this.ViewModel.Mode = mode;
			InitializeComponent();
			this.deliveryNotes.DidOpen += (sender, e) => {
				this.scrollView.ScrollToAsync(this.deliveryNotes, ScrollToPosition.End, true);
			};
			if (mode == ContentMode.Edit && store != null)
			{
				this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonEdit, "", async () =>
					{
						var editStorePage = new SaveStorePage(store);
						await this.Navigation.PushAsync(editStorePage);
					}));
			}
		}

		#endregion

		#region Properties

		private StoreDetailsViewModel _viewModel;

		public StoreDetailsViewModel ViewModel
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
	}
}

