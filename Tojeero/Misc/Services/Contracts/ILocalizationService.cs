using System;
using System.Globalization;

namespace Tojeero.Core.Services
{
	public interface ILocalizationService
	{
		CultureInfo GetCurrentCultureInfo();
	}
}

