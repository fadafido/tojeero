using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Core;

namespace Tojeero.Forms
{
	public partial class BaseCollectionPage : ContentPage
	{
		#region Constructors

		public BaseCollectionPage()
			: base()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties

		private ICollectionViewModel  _viewModel;
		public ICollectionViewModel ViewModel
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

		public object Header
		{
			get
			{
				return this.ListView.Header; 
			}
			set
			{
				this.ListView.Header = value;
			}
		}

		public ListView ListView
		{
			get
			{
				return this.listView;
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

