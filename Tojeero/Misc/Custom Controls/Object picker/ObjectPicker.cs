using System;
using Xamarin.Forms;
using System.Collections.Generic;
using Tojeero.Core;
using System.Linq;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using System.Threading.Tasks;

namespace Tojeero.Forms
{
	public class ObjectPicker<T, CellType> : Entry 
		where T : class
		where CellType : ObjectPickerCell
	{
		#region Private fields and properties

		private ObjectPickerPage<T, CellType> _objectPicker;
		private NavigationPage _objectPickerPage;
		private SelectableViewModel<T>[] _items;

		#endregion

		#region Constructors

		public ObjectPicker()
		{
			this.IsEnabled = false;
			this.HorizontalOptions = LayoutOptions.FillAndExpand;
			this.VerticalOptions = LayoutOptions.FillAndExpand;
			this.GestureRecognizers.Add(new TapGestureRecognizer(async (v) => {
				await showObjectPicker();
			}));
		}

		#endregion

		#region Properties

		private Func<T,T,bool> _comparer;

		public Func<T,T,bool> Comparer
		{
			get
			{
				if (_comparer == null)
				{
					_comparer = (x, y) => x == y;
				}
				return _comparer;
			}
			set
			{
				_comparer = value;
			}
		}

		private Func<T, string> _itemCaption;

		public Func<T, string> ItemCaption
		{
			get
			{
				if (_itemCaption == null)
				{
					_itemCaption = (x) => x != null ? x.ToString() : "";
				}
				return _itemCaption;
			}
			set
			{
				_itemCaption = value;
			}
		}

		#region Items

		public static BindableProperty ItemsProperty = BindableProperty.Create<ObjectPicker<T, CellType>, IList<T>>(o => o.Items, null, propertyChanged: OnItemsChanged);

		public IList<T> Items
		{
			get { return (IList<T>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		private static void OnItemsChanged(BindableObject bindable, IList<T> oldvalue, IList<T> newvalue)
		{
			var picker = bindable as ObjectPicker<T, CellType>;
			if (picker._objectPicker != null)
			{
				picker._items = picker.Items.Select(i => new SelectableViewModel<T>(i, picker.Comparer(i, picker.SelectedItem), picker.ItemCaption)).ToArray();
				picker._objectPicker.ListView.ItemsSource = picker._items;
			}
		}

		#endregion

		#region SelectedItem

		public static BindableProperty SelectedItemProperty = BindableProperty.Create<ObjectPicker<T, CellType>, T>(o => o.SelectedItem, default(T), propertyChanged: OnSelectedItemChanged);

		public T SelectedItem
		{
			get { return (T)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		private static void OnSelectedItemChanged(BindableObject bindable, T oldvalue, T newvalue)
		{
			var picker = bindable as ObjectPicker<T, CellType>;
			if (picker._objectPicker != null)
			{
				if (oldvalue != default(T))
				{
					var oldItem = picker._items.Where(i => picker.Comparer(i.Item, oldvalue)).FirstOrDefault();
					if (oldItem != null)
						oldItem.IsSelected = false;
				}
				if (newvalue != default(T))
				{
					var newItem = picker._items.Where(i => picker.Comparer(i.Item, newvalue)).FirstOrDefault();
					if (newItem != null)
						newItem.IsSelected = true;
				}
			}
			picker.Text = newvalue.ToString();
		}

		#endregion

		#endregion

		#region Utility methods

		private async Task showObjectPicker()
		{
			if (_objectPicker == null)
			{
				_objectPicker = new ObjectPickerPage<T, CellType>();
				_objectPickerPage = new NavigationPage(_objectPicker);
				_objectPicker.ListView.ItemTemplate = new DataTemplate(typeof(CellType));
				_items = this.Items.Select(i => new SelectableViewModel<T>(i, this.Comparer(i,this.SelectedItem), this.ItemCaption)).ToArray();;
				_objectPicker.ListView.ItemsSource = _items;
				_objectPicker.ItemSelected += itemSelected;
			}
			var parent = this.FindParent<Page>();
			await parent.Navigation.PushModalAsync(_objectPickerPage);
		}

		async void itemSelected (object sender, EventArgs<T> e)
		{
			this.SelectedItem = e.Data;
			await this._objectPickerPage.Navigation.PopModalAsync();
		}

		#endregion
	}
}

