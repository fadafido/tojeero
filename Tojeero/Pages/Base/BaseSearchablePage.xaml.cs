using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Core;

namespace Tojeero.Forms
{
	public partial class BaseSearchablePage : ContentPage
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

		public BaseSearchablePage()
			: base()
		{
			InitializeComponent();
			ListView.ItemSelected += storeSelected;
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

		void storeSelected (object sender, SelectedItemChangedEventArgs e)
		{
			((ListView)sender).SelectedItem = null;
		}

		void reloadFinished (object sender, EventArgs e)
		{
			this.ListView.EndRefresh();
		}

		#endregion
	}
}

