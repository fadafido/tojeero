using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core.ViewModels
{
	public class SelectableViewModel<T> : MvxViewModel, ISelectableViewModel
		where T : class
	{
		#region Constructors

		public SelectableViewModel(T item = default(T), bool isSelected = false)
		{
			Item = item;
			IsSelected = isSelected;
		}

		#endregion
		
		#region Properties

		private T _item;

		public T Item
		{ 
			get
			{
				return _item; 
			}
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
			get
			{
				return _isSelected; 
			}
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
			get
			{
				return Item != null ? Item.ToString() : "";
			}
		}

		#endregion
	}
}

