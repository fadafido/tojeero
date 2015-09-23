using System;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Tojeero.Droid;
using Xamarin.Forms;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Droid
{
	public sealed class MvxPagePresenter
		: MvxAndroidViewPresenter, IMvxPageNavigationHost
	{
		public override void Show(MvxViewModelRequest request)
		{
			if (TryShowPage(request))
				return;

			Mvx.Error("Skipping request for {0}", request.ViewModelType.Name);
		}

		private bool TryShowPage(MvxViewModelRequest request)
		{
			if (this.NavigationProvider == null)
				return false;

			var page = MvxPresenterToolbox.CreatePage<Page>(request);
			if (page == null)
				return false;

			var viewModel = MvxPresenterToolbox.LoadViewModel(request);

			page.BindingContext = viewModel;

			this.NavigationProvider.Push(page);

			return true;
		}

		public override void Close(IMvxViewModel viewModel)
		{
			if (this.NavigationProvider == null)
				return;

			this.NavigationProvider.Pop();
		}

		public IMvxPageNavigationProvider NavigationProvider { get; set; }
	}
}

