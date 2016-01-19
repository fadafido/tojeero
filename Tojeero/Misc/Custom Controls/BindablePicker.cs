using System;
using System.Linq;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Generic;

namespace Tojeero.Forms {
	public class BindablePicker<T> : Tojeero.Forms.Picker
	{
		#region Private fields
		private IList<T> objects;
		#endregion

		#region Construtors

		public BindablePicker()
		{
			this.SelectedIndexChanged += OnSelectedIndexChanged;
		}

		#endregion

		#region Properties

		public Func<T, T, bool> Comparer { get; set; }
		public Func<T,string> StringFormat { get; set; }

		#region Items source

		public static BindableProperty ItemsSourceProperty =
			BindableProperty.Create<BindablePicker<T>, IList<T>>(o => o.ItemsSource, null, propertyChanged: OnItemsSourceChanged);


		public IList<T> ItemsSource
		{
			get { return (IList<T>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		private static void OnItemsSourceChanged(BindableObject bindable, IList<T> oldvalue, IList<T> newvalue)
		{			
			var picker = bindable as BindablePicker<T>;
			picker.objects = newvalue;
			picker.Items.Clear();
			if (newvalue != null)
			{
				foreach (var item in picker.objects)
				{
					var value = picker.StringFormat != null ? picker.StringFormat(item) : item.ToString();
					picker.Items.Add(value);
				}
				setSelectedItem(picker, picker.SelectedItem);
			}
		}

		#endregion

		#region Selected item

		public static BindableProperty SelectedItemProperty =
			BindableProperty.Create<BindablePicker<T>, T>(o => o.SelectedItem, default(T), propertyChanged: OnSelectedItemChanged);

		public T SelectedItem
		{
			get { return (T)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		private static void OnSelectedItemChanged(BindableObject bindable, T oldvalue, T newvalue)
		{
			var picker = bindable as BindablePicker<T>;
			setSelectedItem(picker, newvalue);
		}

		#endregion

		#endregion

		#region Utility methods

		private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
		{
			if (!(SelectedIndex < 0 || SelectedIndex > Items.Count - 1))
			{
				SelectedItem = objects[SelectedIndex];
			}
		}


		static void setSelectedItem(BindablePicker<T> picker, T newvalue)
		{
			if (newvalue != null && picker.objects != null)
			{
				if (picker.Comparer == null)
				{ 
					picker.SelectedIndex = picker.objects.IndexOf(newvalue);
				}
				else
				{
					int i = 0;
					foreach (var item in picker.objects)
					{
						if (picker.Comparer(item, newvalue) == true)
							break;
						i++;
					}
					picker.SelectedIndex = i < picker.objects.Count ? i : -1;
				}
			}
			else
			{
				picker.SelectedIndex = -1;
			}
		}
		#endregion
	}
}
