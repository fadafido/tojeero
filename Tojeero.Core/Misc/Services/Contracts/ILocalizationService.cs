using System.Globalization;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.Services.Contracts
{
	public enum LanguageCode
	{
		[StringValue("en")]
		English,
		[StringValue("ar")]
		Arabic,
		[StringValue("unknown")]
		Unknown
	}

	public interface ILocalizationService
	{
		void SetLanguage(LanguageCode language);
		CultureInfo Culture { get; }
		LanguageCode Language { get; }
		string GetNativeLanguageName(LanguageCode language);
	}
}

