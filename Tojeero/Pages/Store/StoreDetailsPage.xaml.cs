using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Core;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class StoreDetailsPage : ContentPage
	{
		
		#region Constructors

		public StoreDetailsPage(IStore store)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<StoreDetailsViewModel>();
			this.ViewModel.Store = store;
			InitializeComponent();
			this.deliveryNotes.DidOpen += (sender, e) => {
				this.scrollView.ScrollToAsync(this.deliveryNotes, ScrollToPosition.End, true);
			};
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

