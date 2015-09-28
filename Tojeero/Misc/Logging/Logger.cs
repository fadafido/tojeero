using System;
using System.Collections.Generic;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using System.Text;
using System.Collections;

namespace Tojeero.Core
{
	public class Logger : ILogger
	{
		#region ILogger implementation

		public void Log(string message, params object[] args)
		{
			Log(message, LoggingLevel.Debug, false, false);
		}

		public void Log(string message, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false, params object[] args)
		{
			Log(null, message, level, logRemotely, args);
		}

		public void Log(Exception ex, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false, params object[] args)
		{
			Log(ex, null, level, logRemotely, args);
		}

		public void Log(Exception ex, string message, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false, params object[] args)
		{
			try
			{
				var details = !string.IsNullOrEmpty(message) ? new Dictionary<string, string> { ["Message" ] = string.Format(message, args) } : null;
				Log(ex, details, level, logRemotely);
			}
			catch (FormatException)
			{
				Mvx.Trace(MvxTraceLevel.Error, "Exception during trace of {0} {1}", level, message);
			}
		}

		public void Log(Exception ex, IDictionary details, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("/------------------------------------------").Append(level.ToString().ToUpper()).Append("------------------------------------------/\n");
			if (details != null && details.Count > 0)
			{
				foreach (var key in details.Keys)
				{
					builder.Append(key).Append(": ").Append(details[key].ToString()).Append("\n");
				}	
			}
			if (ex != null)
				builder.Append(ex.ToString());
			Mvx.Trace(level.ToTraceLevel(), builder.ToString());
			if (logRemotely && level >= LoggingLevel.Warning)
			{
				Xamarin.Insights.Report(ex, details, level.ToSeverity());
			}
		}
		#endregion
		
		
	}
}

