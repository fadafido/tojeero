using System;
using Tojeero.Core;
using Cirrious.CrossCore.Platform;

namespace Tojeero.Core
{
	public static class LoggingExtentions
	{
		public static Xamarin.Insights.Severity ToSeverity(this LoggingLevel level)
		{
			switch (level)
			{
				case LoggingLevel.Error:
					return Xamarin.Insights.Severity.Error;
				case LoggingLevel.Critical:
					return Xamarin.Insights.Severity.Critical;
				default:
					return Xamarin.Insights.Severity.Warning;
			}
		}

		public static MvxTraceLevel ToTraceLevel(this LoggingLevel level)
		{
			switch (level)
			{
				case LoggingLevel.Error:					
				case LoggingLevel.Critical:
					return MvxTraceLevel.Error;
				case LoggingLevel.Warning:
					return MvxTraceLevel.Warning;
				case LoggingLevel.Diagnostic:
					return MvxTraceLevel.Diagnostic;
				default:
					return MvxTraceLevel.Diagnostic;
			}
		}
	}
}

