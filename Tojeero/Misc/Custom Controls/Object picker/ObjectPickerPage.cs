using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Forms.Resources;
using Tojeero.Core;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels;
using System.Threading.Tasks;

namespace Tojeero.Forms
{
	public partial class ObjectPickerPage<T, CellType> : ContentPage 
		where T : class
		where CellType : ObjectPickerCell
	{
		#region Private fields and properties

		private bool _isLoading;
		private bool IsLoading
		{
			get
			{
				return _isLoading;
			}
			set
			{
				if (_isLoading != value)
				{					
					_isLoading = value;
					this.Spinner.IsVisible = _isLoading;
					this.Spinner.IsRunning = _isLoading;
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
			grid.Children.Add(this.Spinner);
			grid.Children.Add(this.ListView);
			this.Content = grid;

			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, null, async () =>
					{
						await this.Navigation.PopModalAsync();
					}));
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
			this.IsLoading = true;
			this.ListView.ItemsSource = await LoaderAction();
			this.IsLoading = false;
		}

		#endregion

		#region Utility methods

		void itemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
			{
				var selected = e.SelectedItem as SelectableViewModel<T>;
				this.ItemSelected.Fire(this, new EventArgs<T>(selected.Item));
			}
			this.ListView.SelectedItem = null;
		}

		#endregion
	}
}

