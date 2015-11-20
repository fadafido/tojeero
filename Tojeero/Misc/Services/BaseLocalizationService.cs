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


		public string GetNativeLanguageName(LanguageCode language)
		{
			var culture = getCultureForLanguage(language);
			return culture.NativeName;
		}

		#endregion

		#region Utility Methods

		protected abstract CultureInfo getSystemCultureInfo ();

		private CultureInfo getCultureForLanguage(LanguageCode language)
		{
			var culture = new CultureInfo(language.GetString());
			return culture;
		}

		private CultureInfo getCurrentCultureInfo()
		{
			CultureInfo culture;
			if (Settings.Language != LanguageCode.Unknown)
			{
				culture = getCultureForLanguage(Settings.Language);
			}
			else
			{
				culture = getSystemCultureInfo();
			}
			return culture ?? getCultureForLanguage(LanguageCode.English);
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
			var newCulture = getCurrentCultureInfo();
			bool isNew = _culture != null && _culture.TwoLetterISOLanguageName != newCulture.TwoLetterISOLanguageName;
			_culture = newCulture;
			_language = getLanguageCode() ?? LanguageCode.English;

			if(isNew)
				_messenger.Publish<LanguageChangedMessage>(new LanguageChangedMessage(this, _language.Value));
		}

		#endregion
	}
}

