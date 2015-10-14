﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class StoresPage : BaseSearchablePage
	{
		#region Properties

		public new StoresViewModel ViewModel
		{
			get
			{
				return base.ViewModel as StoresViewModel;
			}
			set
			{
				base.ViewModel = value;
			}
		}

		#endregion

		#region Constructors

		public StoresPage()
			: base()
		{
			InitializeComponent();
			this.ViewModel = MvxToolbox.LoadViewModel<StoresViewModel>();
			this.SearchBar.Placeholder = AppResources.PlaceholderSearchStores;
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ViewModel.LoadFirstPageCommand.Execute(null);
		}

		#endregion
	}
}

