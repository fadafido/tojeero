using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class BootstrapPage : ContentPage
	{
		#region Properties

		private BoostrapViewModel _viewModel;
		public BoostrapViewModel ViewModel
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

		public BootstrapPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<BoostrapViewModel>();
			InitializeComponent();
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.BootstrapCommand.Execute(null);
		}

		#endregion
	}
}

