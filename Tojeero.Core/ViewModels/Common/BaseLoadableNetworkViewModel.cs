using Connectivity.Plugin;
using Connectivity.Plugin.Abstractions;
using Tojeero.Core.Resources;

namespace Tojeero.Core.ViewModels.Common
{
    public class BaseLoadableNetworkViewModel : BaseLoadableViewModel
    {
        #region Constructors

        public BaseLoadableNetworkViewModel()
        {
            var connectivity = CrossConnectivity.Current;
            connectivity.ConnectivityChanged += handleNetworkConnectionChanged;
            IsNetworkAvailable = connectivity.IsConnected;
        }

        #endregion

        #region Properties

        public static string IsNetworkAvailableProperty = "IsNetworkAvailable";
        private bool _isNetworkAvailable;

        public bool IsNetworkAvailable
        {
            get { return _isNetworkAvailable; }
            set
            {
                if (_isNetworkAvailable != value)
                {
                    _isNetworkAvailable = value;
                    RaisePropertyChanged(() => IsNetworkAvailable);
                    RaisePropertyChanged(() => NoNetworkWarning);
                }
            }
        }

        private static readonly string _noNetworkLabel = AppResources.MessageNoInternetWarning;

        public virtual string NoNetworkWarning
        {
            get { return IsNetworkAvailable ? null : _noNetworkLabel; }
        }

        #endregion

        #region Protected API

        protected virtual void handleNetworkConnectionChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsNetworkAvailable = e.IsConnected;
        }

        #endregion
    }
}