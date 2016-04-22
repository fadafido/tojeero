using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
    public class LoadableViewModel : MvxViewModel, ILoadableViewModel
    {
        #region Constructors

        #endregion

        #region ILoadableViewModel Implementation

        public static string IsLoadingProperty = "IsLoading";
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }


        private string _loadingText;

        public string LoadingText
        {
            get { return _loadingText; }
            set
            {
                _loadingText = value;
                RaisePropertyChanged(() => LoadingText);
            }
        }

        private string _loadingFailureMessage;

        public string LoadingFailureMessage
        {
            get { return _loadingFailureMessage; }
            set
            {
                _loadingFailureMessage = value;
                RaisePropertyChanged(() => LoadingFailureMessage);
            }
        }


        public void StartLoading(string message = "")
        {
            IsLoading = true;
            LoadingFailureMessage = "";
            LoadingText = message;
        }

        public void StopLoading(string failureMessage = "")
        {
            IsLoading = false;
            LoadingText = "";
            LoadingFailureMessage = failureMessage;
        }

        #endregion
    }
}