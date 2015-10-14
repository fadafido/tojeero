using System;

namespace Tojeero.Core.ViewModels
{
	public class BaseSelectableCollectionViewModel<T> : BaseCollectionViewModel<T>
		where T : IModelEntity, ISelectableEntity
	{
		#region Constructors

		public BaseSelectableCollectionViewModel(IModelQuery<T> query, int pageSize = 50)
			: base(query, pageSize)
		{
		}

		#endregion

		#region Properties

		private T _selectedItem;
		public T SelectedItem
		{ 
			get
			{
				return _selectedItem; 
			}
			set
			{
				if (_selectedItem != null)
					_selectedItem.IsSelected = false;
				_selectedItem = value; 
				_selectedItem.IsSelected = true;
				RaisePropertyChanged(() => SelectedItem); 
			}
		}

		#endregion
	}
}

