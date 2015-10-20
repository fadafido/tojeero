﻿using System;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using System.Linq;
using Tojeero.Core.Toolbox;
using Tojeero.Core.Services;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
{
	public class BoostrapViewModel : LoadableNetworkViewModel
	{
		#region Private fields and properties
		private bool _isCachedCleared;
		private bool _isCountriesLoaded;
		private bool _isCitiesLoaded;
		private bool _isBootstrapCompleted;
		#endregion

		#region Constructors

		public BoostrapViewModel()
			: base()
		{
			
		}

		#endregion

		#region Properties

		public Action BootstrapFinished;

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
					}, () => !IsLoading);
				return _bootstrapCommand;
			}
		}

		#endregion

		#region Protected 

		protected override void handleNetworkConnectionChanged(object sender, Connectivity.Plugin.Abstractions.ConnectivityChangedEventArgs e)
		{
			base.handleNetworkConnectionChanged(sender, e);
			if (e.IsConnected && !_isBootstrapCompleted)
				this.BootstrapCommand.Execute(null);
		}

		#endregion

		#region Private API

		private async Task bootstrap()
		{			
			this.StartLoading(AppResources.MessageInitialLoading);
			await clearCache();
			Mvx.Resolve<IAuthenticationService>().RestoreSavedSession();
			
//			if (!(await loadCountries()))
//				return;
//			if (!(await loadCities()))
//				return;
			this.StopLoading();
			_isBootstrapCompleted = true;
			if (BootstrapFinished != null)
				BootstrapFinished();			
		}


		private async Task clearCache()
		{
			if (_isCachedCleared)
				return;
			try
			{
				await Mvx.Resolve<ICacheRepository>().Initialize();
				await Mvx.Resolve<ICacheRepository>().Clear();
				_isCachedCleared = true;
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
				var result = await manager.Fetch();
				if(result.Count() == 0)
				{
					Tools.Logger.Log("Seems there are no registered countries in the backend. Make sure this is not an error!", LoggingLevel.Warning, true);
				}
			}
			catch(OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				StopLoading(AppResources.MessageLoadingTimeOut);
				return false;
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, "Error occurred while loading countries before application launch.", LoggingLevel.Error, true);
				StopLoading(AppResources.MessageLoadingFailed);
				return false;
			}
			_isCountriesLoaded = true;
			return true;
		}

		private async Task<bool> loadCities()
		{
			if (_isCitiesLoaded)
				return true;

			try
			{
				var manager = Mvx.Resolve<ICityManager>();
				var result = await manager.Fetch(0);
				if(result.Count() == 0)
				{
					Tools.Logger.Log("Seems there are no registered cities in the backend. Make sure this is not an error!", LoggingLevel.Warning, true);
				}
			}
			catch(OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				StopLoading(AppResources.MessageLoadingTimeOut);
				return false;
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, "Error occurred while loading cities before application launch.", LoggingLevel.Error, true);
				StopLoading(AppResources.MessageLoadingFailed);
				return false;
			}
			_isCitiesLoaded = true;
			return true;
		}

		#endregion

	}
}
