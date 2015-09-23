using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core.ViewModels
{
	public class LoadableViewModel : MvxViewModel, ILoadableViewModel
	{
		#region Constructors

		public LoadableViewModel()
		{
		}

		#endregion

		#region ILoadableViewModel Implementation

		private bool _isLoading;
		public bool IsLoading
		{ 
			get
			{
				return _isLoading; 
			}
			set
			{
				_isLoading = value; 
				RaisePropertyChanged(() => IsLoading); 
			}
		}


		private string _loadingText;
		public string LoadingText
		{ 
			get
			{
				return _loadingText; 
			}
			set
			{
				_loadingText = value; 
				RaisePropertyChanged(() => LoadingText); 
			}
		}

		private string _loadingFailureMessage;
		public string LoadingFailureMessage
		{ 
			get
			{
				return _loadingFailureMessage; 
			}
			set
			{
				_loadingFailureMessage = value; 
				RaisePropertyChanged(() => LoadingFailureMessage); 
			}
		}


		public void StartLoading(string message = "")
		{
			this.IsLoading = true;
			this.LoadingFailureMessage = "";
			this.LoadingText = message;
		}

		public void StopLoading(string failureMessage = "")
		{
			this.IsLoading = false;
			this.LoadingText = "";
			this.LoadingFailureMessage = failureMessage;
		}

		#endregion
	}
}

