using System.Globalization;
using Tojeero.Core.Model;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.Services.Contracts
{
	public interface ILocalizationService
	{
		void SetLanguage(LanguageCode language);
		CultureInfo Culture { get; }
		LanguageCode Language { get; }
		string GetNativeLanguageName(LanguageCode language);
	}
}

