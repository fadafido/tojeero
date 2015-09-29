using System;
using Cirrious.MvvmCross.ViewModels;
using Parse;
using Cirrious.MvvmCross;
using System.ComponentModel;
using System.Linq.Expressions;
using Cirrious.CrossCore.Core;

namespace Tojeero.Core
{
	public abstract class BaseModelEntity : ParseObject, IModelEntity
	{
		public BaseModelEntity()
		{
			var alwaysOnUIThread = (MvxSingletonCache.Instance == null)
				? true
				: MvxSingletonCache.Instance.Settings.AlwaysRaiseInpcOnUserInterfaceThread;
			ShouldAlwaysRaiseInpcOnUserInterfaceThread(alwaysOnUIThread);
		}

		#region IModelEntity implementation

		public string ID
		{
			get
			{
				return base.ObjectId;
			}
			set
			{
				base.ObjectId = value;
			}
		}

		#endregion

		#region IMvxNotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		private bool _shouldAlwaysRaiseInpcOnUserInterfaceThread;
		public bool ShouldAlwaysRaiseInpcOnUserInterfaceThread()
		{
			return _shouldAlwaysRaiseInpcOnUserInterfaceThread;
		}

		public void ShouldAlwaysRaiseInpcOnUserInterfaceThread(bool value)
		{
			_shouldAlwaysRaiseInpcOnUserInterfaceThread = value;
		}

		public void RaisePropertyChanged<T>(Expression<Func<T>> property)
		{
			var name = this.GetPropertyNameFromExpression(property);
			RaisePropertyChanged(name);
		}

		public void RaisePropertyChanged(string whichProperty)
		{
			var changedArgs = new PropertyChangedEventArgs(whichProperty);
			RaisePropertyChanged(changedArgs);
		}

		public virtual void RaiseAllPropertiesChanged()
		{
			var changedArgs = new PropertyChangedEventArgs(string.Empty);
			RaisePropertyChanged(changedArgs);
		}

		public virtual void RaisePropertyChanged(PropertyChangedEventArgs changedArgs)
		{
			// check for interception before broadcasting change
			if (InterceptRaisePropertyChanged(changedArgs)
				== MvxInpcInterceptionResult.DoNotRaisePropertyChanged) 
				return;

			var raiseAction = new Action(() =>
				{
					var handler = PropertyChanged;

					if (handler != null)
						handler(this, changedArgs);
				});

			if (ShouldAlwaysRaiseInpcOnUserInterfaceThread())
			{
				// check for subscription before potentially causing a cross-threaded call
				if (PropertyChanged == null)
					return;
				Xamarin.Forms.Device.BeginInvokeOnMainThread(raiseAction);
			}
			else
			{
				raiseAction();
			}
		}

		protected virtual MvxInpcInterceptionResult InterceptRaisePropertyChanged(PropertyChangedEventArgs changedArgs)
		{
			if (MvxSingletonCache.Instance != null)
			{
				var interceptor = MvxSingletonCache.Instance.InpcInterceptor;
				if (interceptor != null)
				{
					return interceptor.Intercept(this, changedArgs);
				}
			}

			return MvxInpcInterceptionResult.NotIntercepted;
		}


		#endregion

	}
}

