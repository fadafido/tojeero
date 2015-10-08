using System;
using System.Linq;
using Xamarin.Forms;
using System.Collections;

namespace Tojeero.Forms {
	public class BindablePicker : Tojeero.Forms.Picker
	{
		#region Private fields
		private IList objects;
		#endregion

		#region Construtors

		public BindablePicker()
		{
			this.SelectedIndexChanged += OnSelectedIndexChanged;
		}

		#endregion

		#region Bindable properties

		#region Items source

		public static BindableProperty ItemsSourceProperty =
			BindableProperty.Create<BindablePicker, IList>(o => o.ItemsSource, default(IList), propertyChanged: OnItemsSourceChanged);


		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		private static void OnItemsSourceChanged(BindableObject bindable, IList oldvalue, IList newvalue)
		{			
			var picker = bindable as BindablePicker;
			picker.objects = newvalue;
			picker.Items.Clear();
			if (newvalue != null)
			{
				foreach (var item in picker.objects)
					picker.Items.Add(item.ToString());
				setSelectedItem(picker, picker.SelectedItem);
			}
		}

		#endregion

		#region Selected item

		public static BindableProperty SelectedItemProperty =
			BindableProperty.Create<BindablePicker, object>(o => o.SelectedItem, default(object), propertyChanged: OnSelectedItemChanged);

		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			var picker = bindable as BindablePicker;
			setSelectedItem(picker, newvalue);
		}

		#endregion

		#endregion

		#region Utility methods

		private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
		{
			if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
			{
				SelectedItem = null;
			}
			else
			{
				SelectedItem = objects[SelectedIndex];
			}
		}


		static void setSelectedItem(BindablePicker picker, object newvalue)
		{
			if (newvalue != null && picker.objects != null)
			{
				picker.SelectedIndex = picker.objects.IndexOf(newvalue);
			}
			else
			{
				picker.SelectedIndex = -1;
			}
		}
		#endregion
	}
}
