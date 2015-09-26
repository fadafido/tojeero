﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class SideMenuPage : ContentPage
	{
		#region Properties

		private SideMenuViewModel _viewModel;
		public SideMenuViewModel ViewModel
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

		public SideMenuPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<SideMenuViewModel>();
			this.ViewModel.ShowUserDetails += showUserDetails;
			InitializeComponent();
		}

		#endregion

		#region Utility Methods

		private async void showUserDetails(object sender, EventArgs e)
		{
			await this.Navigation.PushModalAsync(new NavigationPage(new UserDetailsPage()));
		}

		#endregion
	}
}

