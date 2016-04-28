using System;
using Tojeero.Core.ViewModels.Contracts;
using Tojeero.Forms.Controls;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Common
{
    public partial class BaseCollectionPage
    {
        #region Constructors

        public BaseCollectionPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public override ICollectionViewModel ViewModel
        {
            get { return base.ViewModel; }
            protected set
            {
                if (ViewModel != value)
                {
                    DisconnectEvents();
                    base.ViewModel = value;
                    ConnectEvents();
                }
            }
        }

        public DataTemplate ItemTemplate
        {
            get { return ListViewEx.ItemTemplate; }
            set { ListViewEx.ItemTemplate = value; }
        }

        public object Header
        {
            get { return ListViewEx.Header; }
            set { ListViewEx.Header = value; }
        }

        public ListViewEx ListViewEx
        {
            get { return _listViewEx; }
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