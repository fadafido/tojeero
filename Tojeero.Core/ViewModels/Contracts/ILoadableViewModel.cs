using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core.ViewModels.Contracts
{
	public interface ILoadableViewModel : IMvxViewModel
	{
		bool IsLoading { get; set; }
		string LoadingText { get; set; }
		string LoadingFailureMessage { get; set; }
		void StartLoading(string message = "");
		void StopLoading(string failureMessage = "");
	}
}

