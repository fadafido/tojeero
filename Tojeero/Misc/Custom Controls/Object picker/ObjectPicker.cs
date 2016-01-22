using System;
using Xamarin.Forms;
using System.Collections.Generic;
using Tojeero.Core;
using System.Linq;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using System.Threading.Tasks;
using System.Collections;

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
        private TapGestureRecognizer _tapGesture;

        #endregion

        #region Constructors

        public ObjectPicker()
        {
            base.IsEnabled = false;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            _tapGesture = new TapGestureRecognizer(async (v) =>
            {
                await ShowObjectPicker();
            });
            this.GestureRecognizers.Add(_tapGesture);
        }

        #endregion

        #region Properties

        private Func<T, T, bool> _comparer;

        public Func<T, T, bool> Comparer
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

        public Func<Task<IList<T>>> ItemsLoader { get; set; }

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
            picker.Text = picker.ItemCaption(newvalue);
        }

        #endregion

        #region IsEnabled
        public static BindableProperty IsEnabledProperty = BindableProperty.Create<ObjectPicker<T, CellType>, bool>(o => o.IsEnabled, true, propertyChanged: OnIsEnabledChanged);

        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        private static void OnIsEnabledChanged(BindableObject bindable, bool oldvalue, bool newvalue)
        {
            var picker = bindable as ObjectPicker<T, CellType>;
            if (!newvalue && picker.GestureRecognizers.Contains(picker._tapGesture))
                picker.GestureRecognizers.Remove(picker._tapGesture);
            else if (newvalue && !picker.GestureRecognizers.Contains(picker._tapGesture))
                picker.GestureRecognizers.Add(picker._tapGesture);
        } 
        #endregion

        #endregion

        #region Protected methods

        protected virtual async Task ShowObjectPicker()
        {
            if (_objectPicker == null)
            {
                _objectPicker = new ObjectPickerPage<T, CellType>();
                _objectPickerPage = new NavigationPage(_objectPicker);
                _objectPicker.ListView.ItemTemplate = new DataTemplate(typeof(CellType));
                _objectPicker.LoaderAction = async () =>
                {
                    var items = ItemsLoader != null ? await ItemsLoader() : null;
                    _items = items == null ? null :
                        items.Select(i => new SelectableViewModel<T>(i, this.Comparer(i, this.SelectedItem), this.ItemCaption)).ToArray();
                    return _items;
                };
                _objectPicker.ItemSelected += ItemSelected;
            }
            var parent = this.FindParent<Page>();
            await parent.Navigation.PushModalAsync(_objectPickerPage);
        }

        protected virtual async void ItemSelected(object sender, EventArgs<T> e)
        {
            this.SelectedItem = e.Data;
            await this._objectPickerPage.Navigation.PopModalAsync();
            this._objectPickerPage = null;
            this._objectPicker = null;
        }

        #endregion
    }
}

