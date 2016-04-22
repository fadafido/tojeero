using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Tag
{
	public class TagViewModel : MvxViewModel, ISelectableViewModel
	{
		#region Constructors

		public TagViewModel(ITag tag = null)
		{
			Tag = tag;
		}

		#endregion

		#region Properties

		private ITag _tag;

		public ITag Tag
		{ 
			get
			{
				return _tag; 
			}
			set
			{
				_tag = value; 
				RaisePropertyChanged(() => Tag); 
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
				_isSelected = value; 
				RaisePropertyChanged(() => IsSelected); 
			}
		}

		#endregion
	}
}

