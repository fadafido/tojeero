using System;
using System.Collections.Generic;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Contracts;
using Xamarin.Forms;
using ListView = Tojeero.Forms.Controls.ListView;

namespace Tojeero.Forms.Views.Common
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

		public IList<View> Header
		{
			get
			{
				return this.headerContainer.Children; 
			}
			set
			{
				this.headerContainer.Children.Clear();
				this.headerContainer.Children.AddRange(value);
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

		#endregion
	}
}

