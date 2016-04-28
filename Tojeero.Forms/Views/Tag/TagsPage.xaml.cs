using System;
using Tojeero.Core;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Tag;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Tag
{
    public partial class TagsPage
    {
        #region Properties

        public event EventHandler<EventArgs<string>> DidClose;

        public new TagsViewModel ViewModel
        {
            get { return base.ViewModel as TagsViewModel; }
            set { base.ViewModel = value; }
        }
        #endregion

        #region Constructors

        public TagsPage()
        {
            InitializeComponent();
            ViewModel = MvxToolbox.LoadViewModel<TagsViewModel>();
            ViewModel.CloseAction = async () => await Navigation.PopModalAsync();
            ViewModel.SelectTagAction = t => DidClose.Fire(this, new EventArgs<string>(t));
           

            SearchBar.Placeholder = "Search for tags";
            ListViewEx.RowHeight = 50;
            ListViewEx.ItemSelected += ItemSelected;
            
            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, "",
                async () => { await Navigation.PopModalAsync(); }));
        }

        #endregion

        #region Utility methods

        private void ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
                ViewModel.ViewModel.SelectedItem = e.SelectedItem as TagViewModel;
            ListViewEx.SelectedItem = null;
        }


        #endregion
    }
}