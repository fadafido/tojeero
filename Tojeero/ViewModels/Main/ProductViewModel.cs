using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Tojeero.Core.ViewModels
{
	public class ProductViewModel : BaseUserViewModel
	{
		#region Private fields and properties

		AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public ProductViewModel(IProduct product = null)
			:base(Mvx.Resolve<IAuthenticationService>(), Mvx.Resolve<IMvxMessenger>())
		{
			this.Product = product;
			this.PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		private IProduct _product;
		public IProduct Product
		{ 
			get
			{
				return _product; 
			}
			set
			{
				_product = value; 
				RaisePropertyChanged(() => Product); 
			}
		}
			
		private static string IsFavoriteProperty = "IsFavorite";
		private bool? _isFavorite;
		public bool? IsFavorite
		{ 
			get
			{
				return _isFavorite; 
			}
			set
			{
				_isFavorite = value; 
				RaisePropertyChanged(() => IsFavorite); 
			}
		}
		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadFavoriteCommand;
		public System.Windows.Input.ICommand LoadFavoriteCommand
		{
			get
			{
				_loadFavoriteCommand = _loadFavoriteCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>{
					await loadFavorite();
				}, () => CanExecuteLoadFavoriteCommand);
				return _loadFavoriteCommand;
			}
		}

		public bool CanExecuteLoadFavoriteCommand
		{
			get
			{
				return this.IsNetworkAvailable && this.IsLoggedIn;
			}
		}


		private Cirrious.MvvmCross.ViewModels.MvxCommand _toggleFavoriteCommand;
		public System.Windows.Input.ICommand TogglFavoriteCommand
		{
			get
			{
				_toggleFavoriteCommand = _toggleFavoriteCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
					await toggleFavorite();
				}, () => CanExecuteToggleFavoriteCommand);
				return _toggleFavoriteCommand;
			}
		}

		public bool CanExecuteToggleFavoriteCommand
		{
			get
			{
				return this.IsFavorite != null && this.IsNetworkAvailable && !IsLoading && this.IsLoggedIn;
			}
		}

		#endregion

		#region Utility methods

		private static Random favRand = new Random();
		private async Task loadFavorite()
		{
			StartLoading();
			using (var writerLock = await _locker.WriterLockAsync())
			{
				if (this.IsFavorite != null)
					return;
				await Task.Delay(1000);
				this.IsFavorite = favRand.Next() % 2 == 0;
			}
			StopLoading();
		}
			
		private async Task toggleFavorite()
		{
			StartLoading();
			using (var writerLock = await _locker.WriterLockAsync())
			{
				await Task.Delay(1000);
				this.IsFavorite = !this.IsFavorite.Value;
			}
			StopLoading();
		}

		private async void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty)
			{				
				this.RaisePropertyChanged(() => CanExecuteLoadFavoriteCommand);
				this.RaisePropertyChanged(() => CanExecuteToggleFavoriteCommand);
			}
			else if (e.PropertyName == IsFavoriteProperty || e.PropertyName == IsLoadingProperty)
			{
				this.RaisePropertyChanged(() => CanExecuteToggleFavoriteCommand);
			}

			//If the network state has changed to online and the favorite has not been loaded reload it
			if (e.PropertyName == IsNetworkAvailableProperty && this.IsNetworkAvailable && IsFavorite == null)
			{
				this.LoadFavoriteCommand.Execute(null);
			}

			//If the user state has changed to logged off then we need to clean the favorite state
			if (e.PropertyName == IsLoggedInProperty && !this.IsLoggedIn)
			{
				this.IsFavorite = null;
			}
		}

		#endregion
	}
}

