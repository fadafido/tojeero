using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class StoresPage : ContentPage
	{
		#region Properties

		private StoresViewModel _viewModel;
		public StoresViewModel ViewModel
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

		#region Constructors

		public StoresPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<StoresViewModel>();
			InitializeComponent();
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.LoadNextPageCommand.Execute(null);
		}

		#endregion
	}
}

