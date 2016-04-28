using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
    public class BaseSelectableViewModel<T> : MvxViewModel, ISelectableViewModel
        where T : class
    {
        #region Constructors

        public BaseSelectableViewModel(T item = default(T), bool isSelected = false, Func<T, string> itemCaption = null)
        {
            Item = item;
            IsSelected = isSelected;
            ItemCaption = itemCaption;
        }

        #endregion

        #region Properties

        private T _item;

        public T Item
        {
            get { return _item; }
            set
            {
                _item = value;
                RaisePropertyChanged(() => Item);
                RaisePropertyChanged(() => Caption);
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged(() => IsSelected);
                }
            }
        }

        public string Caption
        {
            get { return ItemCaption(Item); }
        }

        private Func<T, string> _itemCaption;

        public Func<T, string> ItemCaption
        {
            get
            {
                if (_itemCaption == null)
                {
                    _itemCaption = x => x != null ? x.ToString() : "";
                }
                return _itemCaption;
            }
            set { _itemCaption = value; }
        }

        #endregion
    }
}