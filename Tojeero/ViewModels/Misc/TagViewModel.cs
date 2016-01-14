using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
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

