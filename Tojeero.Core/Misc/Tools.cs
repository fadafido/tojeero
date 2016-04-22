using System;
using Cirrious.CrossCore;
using Tojeero.Core.Logging;

namespace Tojeero.Core
{
	public static class Tools
	{
		private static ILogger LoggerInstance = null;
		public static ILogger Logger
		{
			get
			{
				if (LoggerInstance == null)
				{
					LoggerInstance = Mvx.Resolve<ILogger>();
				}
				return LoggerInstance;
			}
		}
	}
}

