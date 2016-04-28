using System;
using System.Collections.Generic;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Contracts;
using Tojeero.Forms.Controls;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Common
{
    public partial class BaseSearchablePage
    {
        #region Properties

        public ISearchViewModel ViewModel
        {
            get { return base.ViewModel; }
            set
            {
                if (ViewModel != value)
                {
                    DisconnectEvents();
                    base.ViewModel = value;
                    ConnectEvents();
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
                ViewModel.ReloadFinished += ReloadFinished;
            }
        }

        protected virtual void DisconnectEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ReloadFinished -= ReloadFinished;
            }
        }

        #endregion

        #region Event Handlers

        void ReloadFinished(object sender, EventArgs e)
        {
            ListViewEx.EndRefresh();
        }

        #endregion
    }
}