using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class UserDetailsPage : ContentPage
	{
		#region Properties

		private UserDetailsViewModel _viewModel;
		public UserDetailsViewModel ViewModel
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

		public UserDetailsPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<UserDetailsViewModel>();
			this.ViewModel.Close += closeUserDetails;
			InitializeComponent();
		}

		#endregion

		#region Utility Methods

		private async void closeUserDetails(object sender, EventArgs e)
		{
			await this.Navigation.PopModalAsync();
		}

		#endregion
	}
}

