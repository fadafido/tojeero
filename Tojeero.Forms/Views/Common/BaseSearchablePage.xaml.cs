using System;
using System.Collections.Generic;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Contracts;
using Tojeero.Forms.Controls;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Common
{
    public partial class BaseSearchablePage : ContentPage
    {
        #region Properties

        private ISearchViewModel _viewModel;

        public ISearchViewModel ViewModel
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

        #endregion

        #region Constructors

        public BaseSearchablePage()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public DataTemplate ItemTemplate
        {
            get { return ListViewEx.ItemTemplate; }
            set { ListViewEx.ItemTemplate = value; }
        }

        public ListViewEx ListViewEx
        {
            get { return _listViewEx; }
        }

        public SearchBar SearchBar
        {
            get { return searchBar; }
        }

        public IList<View> Header
        {
            get { return headerContainer.Children; }
            set
            {
                headerContainer.Children.Clear();
                headerContainer.Children.AddRange(value);
            }
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
            ListViewEx.EndRefresh();
        }

        #endregion
    }
}