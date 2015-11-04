using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core.ViewModels
{
	public class FavoriteSocialViewModel : MvxViewModel
	{
		private ISocialObject _socialObject;

		public ISocialObject SocialObject
		{ 
			get
			{
				return _socialObject; 
			}
			set
			{
				_socialObject = value; 
				RaisePropertyChanged(() => SocialObject); 
			}
		}

		private ISocialViewModel _socialViewModel;

		public ISocialViewModel SocialViewModel
		{ 
			get
			{
				return _socialViewModel; 
			}
			set
			{
				_socialViewModel = value; 
				RaisePropertyChanged(() => SocialViewModel); 
			}
		}
	}
}

