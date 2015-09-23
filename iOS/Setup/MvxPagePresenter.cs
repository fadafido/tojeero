using UIKit;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Xamarin.Forms;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using System.Threading.Tasks;
using Tojeero.Forms.Toolbox;

namespace Tojeero.iOS
{
	public sealed class MvxPagePresenter : IMvxTouchViewPresenter
	{
		private readonly UIWindow _window;
		private NavigationPage _navigationPage;

		public MvxPagePresenter(UIWindow window)
		{
			_window = window;
		}

		public async void Show(MvxViewModelRequest request)
		{
			if (await TryShowPage(request))
				return;

			Mvx.Error("Skipping request for {0}",
				request.ViewModelType.Name);
		}

		private async Task<bool> TryShowPage(MvxViewModelRequest request)
		{
			var page = MvxPresenterToolbox.CreatePage<Page>(request);
			if (page == null)
				return false;

			var viewModel = MvxPresenterToolbox.LoadViewModel(request);

			if (_navigationPage == null)
			{
				_navigationPage = new NavigationPage(page);
				_window.RootViewController =
					_navigationPage.CreateViewController();
			}
			else
			{
				await _navigationPage.PushAsync(page);
			}

			page.BindingContext = viewModel;
			return true;
		}

		public async void ChangePresentation(MvxPresentationHint hint)
		{
			if (hint is MvxClosePresentationHint)
			{
				await _navigationPage.PopAsync();
			}
		}

		public bool PresentModalViewController(UIViewController controller,
			bool animated)
		{
			return false;
		}

		public void NativeModalViewControllerDisappearedOnItsOwn()
		{

		}
	}
}