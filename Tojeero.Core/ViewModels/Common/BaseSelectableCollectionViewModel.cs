using Tojeero.Core.Model.Contracts;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
	public class BaseSelectableCollectionViewModel<T> : BaseCollectionViewModel<T>
		where T : ISelectableViewModel
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

