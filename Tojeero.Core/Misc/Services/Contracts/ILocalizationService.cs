using System.Globalization;
using Tojeero.Core.Model;

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