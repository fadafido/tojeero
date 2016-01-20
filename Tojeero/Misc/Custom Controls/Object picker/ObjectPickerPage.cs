using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Forms.Resources;
using Tojeero.Core;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels;

namespace Tojeero.Forms
{
	public partial class ObjectPickerPage<T, CellType> : ContentPage 
		where T : class
		where CellType : ObjectPickerCell
	{
		#region Constructors

		public ObjectPickerPage()
		{
			this.Content = this.ListView;
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
					_listView.SeparatorVisibility = SeparatorVisibility.None;
					_listView.HorizontalOptions = LayoutOptions.FillAndExpand;
					_listView.VerticalOptions = LayoutOptions.FillAndExpand;
				}
				return _listView;
			}
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

