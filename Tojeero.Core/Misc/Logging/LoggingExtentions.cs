using Cirrious.CrossCore.Platform;
using Xamarin;

namespace Tojeero.Core.Logging
{
    public static class LoggingExtentions
    {
        public static Insights.Severity ToSeverity(this LoggingLevel level)
        {
            switch (level)
            {
                case LoggingLevel.Error:
                    return Insights.Severity.Error;
                case LoggingLevel.Critical:
                    return Insights.Severity.Critical;
                default:
                    return Insights.Severity.Warning;
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