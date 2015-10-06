using System;
using System.Globalization;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.Services
{
	public enum LanguageCode
	{
		[StringValue("en")]
		English,
		[StringValue("ar")]
		Arabic
	}

	public interface ILocalizationService
	{
		void SetLanguage(LanguageCode language);
		CultureInfo Culture { get; }
		LanguageCode Language { get; }
	}
}

