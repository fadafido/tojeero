using System;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using System.Linq;
using Tojeero.Core.Toolbox;
using Tojeero.Core.Services;

namespace Tojeero.Core.ViewModels
{
	public class BoostrapViewModel : LoadableNetworkViewModel
	{
		#region Private fields and properties
		private bool _isCachedCleared;
		private bool _isCountriesLoaded;
		#endregion

		#region Constructors

		public BoostrapViewModel()
			: base()
		{
			
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs> BootstrapFinished;

		private bool _isBootstrapFailed;
		public bool IsBootstrapFailed
		{ 
			get
			{
				return _isBootstrapFailed; 
			}
			set
			{
				_isBootstrapFailed = value; 
				RaisePropertyChanged(() => IsBootstrapFailed); 
			}
		}
			
		#endregion


		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _bootstrapCommand;

		public System.Windows.Input.ICommand BootstrapCommand
		{
			get
			{
				_bootstrapCommand = _bootstrapCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await bootstrap();
					}, () => !IsLoading && IsNetworkAvailable);
				return _bootstrapCommand;
			}
		}

		#endregion

		#region Protected 

		protected override void handleNetworkConnectionChanged(object sender, Connectivity.Plugin.Abstractions.ConnectivityChangedEventArgs e)
		{
			base.handleNetworkConnectionChanged(sender, e);
			if (IsBootstrapFailed)
				this.BootstrapCommand.Execute(null);
		}

		#endregion

		#region Private API

		private async Task bootstrap()
		{
			this.StartLoading("Loading initial data, please wait.");
			await Task.Delay(2000);
			await Mvx.Resolve<IAuthenticationService>().RestoreSavedSession();
			await clearCache();
			if (!(await loadCountries()))
				return;
			this.StopLoading();
			BootstrapFinished.Fire(this, new EventArgs());
		}


		private async Task clearCache()
		{
			if (_isCachedCleared)
				return;
			try
			{
				await Mvx.Resolve<ICacheRepository>().Initialize();
				await Mvx.Resolve<ICacheRepository>().Clear();
			}
			catch(OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, "Error occurred while clearing local cache before application launch.", LoggingLevel.Error, true);
			}
		}

		private bool test = false;
		private async Task<bool> loadCountries()
		{
			//Check if user has already selected the country do not load countries
			if (Settings.CountryId != null)
				return true;

			if (_isCountriesLoaded)
				return true;

			try
			{
				var manager = Mvx.Resolve<ICountryManager>();
				var result = await manager.FetchCountries();
				if(result.Count() == 0)
				{
					Tools.Logger.Log("Seems there are no registered countries in the backend. Make sure this is not an error!", LoggingLevel.Warning, true);
				}
			}
			catch(OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				StopLoading("Failed to load countries because of timeout. Please try again.");
				return false;
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, "Error occurred while loading countries before application launch.", LoggingLevel.Error, true);
				StopLoading("Failed to load countries because of unknown error. Please try again. If the issue persists please contact our support.");
				return false;
			}
			_isCountriesLoaded = true;
			return true;
		}

		#endregion

	}
}

