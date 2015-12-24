using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class BaseSearchableTabPage : ContentPage
	{
		#region Properties

		private ISearchViewModel _viewModel;
		public ISearchViewModel ViewModel
		{
			get
			{
				return _viewModel;
			}
			set
			{
				if (_viewModel != value)
				{
					DisconnectEvents();
					_viewModel = value;
					ConnectEvents();
					this.BindingContext = _viewModel;
				}
			}
		}

		#endregion

		#region Constructors

		public BaseSearchableTabPage()
		{
			InitializeComponent();
			NavigationPage.SetTitleIcon(this, "tojeero.png");
		}
			
		#endregion

		#region Properties

		public DataTemplate ItemTemplate
		{ 
			get
			{
				return this.ListView.ItemTemplate; 
			}
			set
			{
				this.ListView.ItemTemplate = value;
			}
		}

		public ListView ListView
		{
			get
			{
				return this.listView;
			}
		}

		public SearchBar SearchBar
		{
			get
			{
				return this.searchBar;
			}
		}

		public TabButton ProductsButton
		{
			get
			{
				return this.productsTabButton;
			}
		}

		public TabButton StoresButton
		{
			get
			{
				return this.storesTabButton;
			}
		}
		#endregion

		#region Protected API

		protected virtual void ConnectEvents()
		{
			if (this.ViewModel != null)
			{
				this.ViewModel.ReloadFinished += reloadFinished;
			}
		}

		protected virtual void DisconnectEvents()
		{
			if (this.ViewModel != null)
			{
				this.ViewModel.ReloadFinished -= reloadFinished;
			}
		}

		#endregion

		#region Event Handlers

		void reloadFinished (object sender, EventArgs e)
		{
			this.ListView.EndRefresh();
		}

		protected void productButtonClicked(object sender, EventArgs e)
		{
			var root = this.FindParentPage<RootPage>();
			if (root != null)
			{
				root.SelectProductsPage();
			}
		}

		protected void storeButtonClicked(object sender, EventArgs e)
		{
			var root = this.FindParentPage<RootPage>();
			if (root != null)
			{
				root.SelectStoresPage();
			}
		}

		#endregion
	}
}

