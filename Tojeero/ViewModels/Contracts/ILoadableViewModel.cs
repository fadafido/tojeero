using System;
using Cirrious.MvvmCross.ViewModels;
using System.Windows.Input;

namespace Tojeero.Core.ViewModels
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

