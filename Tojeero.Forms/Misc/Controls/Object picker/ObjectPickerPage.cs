using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public class ObjectPickerPage<T, CellType> : ContentPage
        where T : class
        where CellType : ObjectPickerCell
    {
        #region Private fields and properties

        private bool _isLoading;

        private bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    Spinner.IsVisible = _isLoading;
                    Spinner.IsRunning = _isLoading;
                }
            }
        }

        private ActivityIndicator _spinner;

        private ActivityIndicator Spinner
        {
            get
            {
                if (_spinner == null)
                {
                    _spinner = new ActivityIndicator();
                    _spinner.HorizontalOptions = LayoutOptions.Center;
                    _spinner.VerticalOptions = LayoutOptions.Center;
                }
                return _spinner;
            }
        }

        #endregion

        #region Constructors

        public ObjectPickerPage()
        {
            var grid = new Grid();
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.VerticalOptions = LayoutOptions.FillAndExpand;
            grid.Children.Add(Spinner);
            grid.Children.Add(ListView);
            Content = grid;

            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, null,
                async () => { await Navigation.PopModalAsync(); }));
        }

        #endregion

        #region Properties

        public event EventHandler<EventArgs<T>> ItemSelected;

        private ListView _listView;

        public ListView ListView
        {
            get
            {
                if (_listView == null)
                {
                    _listView = new ListView();
                    _listView.ItemSelected += itemSelected;
                    _listView.BackgroundColor = Color.Transparent;
                    _listView.SeparatorVisibility = SeparatorVisibility.None;
                    _listView.HorizontalOptions = LayoutOptions.FillAndExpand;
                    _listView.VerticalOptions = LayoutOptions.FillAndExpand;
                }
                return _listView;
            }
        }

        public Func<Task<IEnumerable<SelectableViewModel<T>>>> LoaderAction { get; set; }

        #endregion

        #region View lifecycle management

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            IsLoading = true;
            ListView.ItemsSource = await LoaderAction();
            IsLoading = false;
        }

        #endregion

        #region Utility methods

        void itemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var selected = e.SelectedItem as SelectableViewModel<T>;
                ItemSelected.Fire(this, new EventArgs<T>(selected.Item));
            }
            ListView.SelectedItem = null;
        }

        #endregion
    }
}