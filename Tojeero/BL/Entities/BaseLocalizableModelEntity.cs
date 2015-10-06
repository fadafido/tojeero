using System;
using Parse;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;

namespace Tojeero.Core
{
	public abstract class BaseLocalizableModelEntity<T> : BaseModelEntity<T> 
		where T: ParseObject
	{
		#region Private fields

		private ILocalizationService _localization;
		private IMvxMessenger _messenger;
		private MvxSubscriptionToken _token;

		#endregion

		#region Constructors

		public BaseLocalizableModelEntity()
			:base()
		{
			initialize();
		}

		public BaseLocalizableModelEntity(T parseObject = null)
			: base(parseObject)
		{
			initialize();
		}

		private void initialize()
		{
			_localization = Mvx.Resolve<ILocalizationService>();
			_messenger = Mvx.Resolve<IMvxMessenger>();
			_token = _messenger.Subscribe<CultureChangedMessage>((message) =>
				{
					raiseCulturalPropertyChange();
				});
		}

		#endregion

		#region Properties

		public LanguageCode Language
		{
			get
			{
				return _localization.Language;
			}
		}

		#endregion

		#region Protected api

		protected abstract void raiseCulturalPropertyChange();

		#endregion
	}
}

