using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Windows.Input;
using Tojeero.Core;
using Tojeero.Core.ViewModels;

namespace Tojeero.Forms
{
	public partial class FavoriteSocialView : StackLayout
	{
		#region Constructors

		public FavoriteSocialView()
		{			
			InitializeComponent();
			this.ViewModel = new FavoriteSocialViewModel();
		}

		#endregion

		#region Properties

		private FavoriteSocialViewModel _viewModel;

		public FavoriteSocialViewModel ViewModel
		{ 
			get
			{
				return _viewModel; 
			}
			private set
			{
				_viewModel = value; 
				this.rootContent.BindingContext = value;
			}
		}

		#endregion

		#region Bindable properties

		#region SocialObject

		public static BindableProperty SocialObjectProperty = BindableProperty.Create<FavoriteSocialView, ISocialObject>(o => o.SocialObject, null, propertyChanged: OnSocialObjectChanged);

		public ISocialObject SocialObject
		{
			get { return (ISocialObject)GetValue(SocialObjectProperty); }
			set { SetValue(SocialObjectProperty, value); }
		}

		private static void OnSocialObjectChanged(BindableObject bindable, ISocialObject oldvalue, ISocialObject newvalue)
		{
			var view = (FavoriteSocialView)bindable;
			view.ViewModel.SocialObject = newvalue;
		}

		#endregion

		#region Social view model

		public static BindableProperty SocialViewModelProperty = BindableProperty.Create<FavoriteSocialView, ISocialViewModel>(o => o.SocialViewModel, null, propertyChanged: OnSocialViewModelChanged);

		public ISocialViewModel SocialViewModel
		{
			get { return (ISocialViewModel)GetValue(SocialViewModelProperty); }
			set { SetValue(SocialViewModelProperty, value); }
		}

		private static void OnSocialViewModelChanged(BindableObject bindable, ISocialViewModel oldvalue, ISocialViewModel newvalue)
		{
			var view = (FavoriteSocialView)bindable;
			view.ViewModel.SocialViewModel = newvalue;
		}

		#endregion

		#endregion
	}
}

