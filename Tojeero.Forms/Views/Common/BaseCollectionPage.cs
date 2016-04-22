using System;
using Tojeero.Core.ViewModels.Contracts;
using Xamarin.Forms;
using ListView = Tojeero.Forms.Controls.ListView;

namespace Tojeero.Forms.Views.Common
{
    public partial class BaseCollectionPage : ContentPage
    {
        #region Constructors

        public BaseCollectionPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private ICollectionViewModel _viewModel;

        public ICollectionViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    DisconnectEvents();
                    _viewModel = value;
                    ConnectEvents();
                    BindingContext = _viewModel;
                }
            }
        }

        public DataTemplate ItemTemplate
        {
            get { return ListView.ItemTemplate; }
            set { ListView.ItemTemplate = value; }
        }

        public object Header
        {
            get { return ListView.Header; }
            set { ListView.Header = value; }
        }

        public ListView ListView
        {
            get { return listView; }
        }

        #endregion

        #region Protected API

        protected virtual void ConnectEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ReloadFinished += reloadFinished;
            }
        }

        protected virtual void DisconnectEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ReloadFinished -= reloadFinished;
            }
        }

        #endregion

        #region Event Handlers

        void reloadFinished(object sender, EventArgs e)
        {
            ListView.EndRefresh();
        }

        #endregion
    }
}