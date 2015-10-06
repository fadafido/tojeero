using System;
using System.Globalization;
using Tojeero.Core.Toolbox;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using Tojeero.Core.Messages;

namespace Tojeero.Core.Services
{
	
	public abstract class BaseLocalizationService : ILocalizationService
	{
		#region Private fields and properties

		private readonly IMvxMessenger _messenger;

		#endregion

		#region Constructors

		public BaseLocalizationService()
		{
			_messenger = Mvx.Resolve<IMvxMessenger>();
		}

		#endregion

		#region ILocalizationService implementation

		public void SetLanguage(LanguageCode language)
		{
			Settings.Language = language;
			reloadCurrentCulture();
		}

		private CultureInfo _culture;
		public CultureInfo Culture
		{
			get
			{
				if (_culture == null)
				{
					_culture = getCurrentCultureInfo();
				}
				return _culture;
			}
		}

		private LanguageCode? _language;
		public LanguageCode Language
		{
			get
			{
				if (_language == null)
				{
					_language = getLanguageCode();
				}
				return _language ?? LanguageCode.English;
			}
		}

		#endregion

		#region Utility Methods

		protected abstract CultureInfo getSystemCultureInfo ();

		private CultureInfo getCurrentCultureInfo()
		{
			CultureInfo culture;
			if (Settings.Language != null)
			{
				culture = getCultureInfoForLanguage(Settings.Language.Value);
			}
			else
			{
				culture = getSystemCultureInfo();
			}
			return culture;
		}

		private CultureInfo getCultureInfoForLanguage(LanguageCode language)
		{
			var culture = new CultureInfo(language.GetString());
			return culture;
		}

		private LanguageCode? getLanguageCode()
		{
			if (_culture == null)
				return null;
			var culture = this._culture.TwoLetterISOLanguageName;
			try
			{
				var language = culture.GetEnum<LanguageCode>();
				return language;
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, "Error occured when loading language code from string.", LoggingLevel.Error, true);
			}
			return null;
		}

		public void reloadCurrentCulture()
		{
			_culture = getCurrentCultureInfo();
			_language = getLanguageCode() ?? LanguageCode.English;
			_messenger.Publish<CultureChangedMessage>(new CultureChangedMessage(this, _culture));
		}

		#endregion
	}
}

