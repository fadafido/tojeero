using System;
using System.Collections.Generic;
using System.Collections;

namespace Tojeero.Core
{
	public interface ILogger
	{
		/// <summary>
		/// Log the specified message by substituting parameters with args.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="args">Arguments.</param>
		void Log(string message, params object[] args);

		/// <summary>
		/// Log the specified message for specified logging level by substituting parameters with args. 
		/// If logRemotely is set to true, Logger will try to log event remotely. 
		/// Only messages with LogginLevel.Warning and up will be logged remotely.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="level">Level.</param>
		/// <param name="logRemotely">If set to <c>true</c> log remotely.</param>
		/// <param name="args">Arguments.</param>
		void Log(string message, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false, params object[] args);

		/// <summary>
		/// Log the specified exception with provided details message for specified logging level by substituting parameters with args.
		/// If logRemotely is set to true, Logger will try to log event remotely. 
		/// Only messages with LogginLevel.Warning and up will be logged remotely.
		/// </summary>
		/// <param name="ex">Exception.</param>
		/// <param name="message">Message.</param>
		/// <param name="level">Level.</param>
		/// <param name="logRemotely">If set to <c>true</c> log remotely.</param>
		/// <param name="args">Arguments.</param>
		void Log(Exception ex, string message,  LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false, params object[] args);

		/// <summary>
		/// Log the specified exception for specified logging level by substituting parameters with args..
		/// If logRemotely is set to true, Logger will try to log event remotely. 
		/// Only messages with LogginLevel.Warning and up will be logged remotely.
		/// </summary>
		/// <param name="ex">Exception.</param>
		/// <param name="level">Logging level.</param>
		/// <param name="logRemotely">If set to <c>true</c> log remotely.</param>
		/// <param name="args">Arguments.</param>
		void Log(Exception ex, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false);

		/// <summary>
		/// Log the specified exception with provided details dictionary and logging level.
		/// If logRemotely is set to true, Logger will try to log event remotely. 
		/// Only messages with LogginLevel.Warning and up will be logged remotely.
		/// </summary>
		/// <param name="ex">Exception</param>
		/// <param name="details">Details dictionary.</param>
		/// <param name="level">Logging level.</param>
		/// <param name="logRemotely">If set to <c>true</c> log remotely.</param>
		void Log(Exception ex, IDictionary details, LoggingLevel level = LoggingLevel.Debug, bool logRemotely = false);
	}
}

