﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Xamarin;

namespace Tojeero.Core.Logging
{
    public class Logger : ILogger
    {
        #region ILogger implementation

        public void Log(string message, params object[] args)
        {
            Mvx.Trace(message, args);
        }

        public void Log(string message, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false,
            params object[] args)
        {
            Log(null, message, level, logRemotely, args);
        }

        public void Log(Exception ex, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false)
        {
            Log(ex, null, level, logRemotely, null);
        }

        public void Log(Exception ex, string message, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false,
            params object[] args)
        {
            try
            {
                var details = !string.IsNullOrEmpty(message)
                    ? new Dictionary<string, string> {["Message"] = string.Format(message, args)}
                    : null;
                Log(ex, details, level, logRemotely);
            }
            catch (FormatException)
            {
                Mvx.Trace(MvxTraceLevel.Error, "Exception during trace of {0} {1}", level, message);
            }
        }

        public void Log(Exception ex, IDictionary details, LoggingLevel level = LoggingLevel.Debug,
            bool logRemotely = false)
        {
            var builder = new StringBuilder();
            builder.Append("/------------------------------------------")
                .Append(level.ToString().ToUpper())
                .Append("------------------------------------------/\n");
            if (details != null && details.Count > 0)
            {
                foreach (var key in details.Keys)
                {
                    builder.Append(key).Append(": ").Append(details[key]).Append("\n");
                }
            }
            if (ex != null)
                builder.Append(ex);
            Mvx.Trace(level.ToTraceLevel(), builder.ToString());
            if (logRemotely && level >= LoggingLevel.Warning)
            {
                Insights.Report(ex, details, level.ToSeverity());
            }
        }

        #endregion
    }
}