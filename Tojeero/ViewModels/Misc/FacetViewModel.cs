using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core.ViewModels
{
	public class FacetViewModel<T> : MvxViewModel where T : IUniqueEntity
	{
		#region Constructors

		public FacetViewModel(T data = default(T), int count = 0)
		{
			Data = data;
			Count = count;
		}

		#endregion

		#region Properties

		private T _data;

		public T Data
		{ 
			get
			{
				return _data; 
			}
			set
			{
				_data = value; 
				RaisePropertyChanged(() => Data); 
			}
		}

		private int _count;

		public int Count
		{ 
			get
			{
				return _count; 
			}
			set
			{
				_count = value; 
				RaisePropertyChanged(() => Count); 
			}
		}

		#endregion
	}
}

