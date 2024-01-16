using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace XOutput.Logging
{
	/// <summary>
	/// Logger base class.
	/// </summary>
	public abstract class AbstractLogger : ILogger
	{
		private readonly Type loggerType;
		public Type LoggerType => loggerType;
		private readonly int level;
		public int Level => level;

		protected AbstractLogger(Type loggerType, int level)
		{
			this.loggerType = loggerType;
			this.level = level;
		}

		protected string CreateLogEntryMessage(DateTime time, LogLevel loglevel, string? classname, string? methodname, string message)
		{
			return $"{time.ToString("yyyy-MM-dd HH\\:mm\\:ss.fff zzz")} {loglevel.Text} {classname}.{methodname}: {message}";
		}

		public Task Trace(string log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Trace, methodName, log);
		}

		public Task Trace(Func<string> log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Trace, methodName, log);
		}

		public Task Debug(string log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Debug, methodName, log);
		}

		public Task Debug(Func<string> log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Debug, methodName, log);
		}

		public Task Info(string log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Info, methodName, log);
		}

		public Task Info(Func<string> log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Info, methodName, log);
		}

		public Task Warning(string log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Warning, methodName, log);
		}

		public Task Warning(Func<string> log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Warning, methodName, log);
		}

		public Task Warning(Exception ex, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Warning, methodName, ex.ToString());
		}

		public Task Error(string log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Error, methodName, log);
		}

		public Task Error(Func<string> log, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Error, methodName, log);
		}

		public Task Error(Exception ex, [CallerMemberName] string? methodName = null)
		{
			return LogCheck(LogLevel.Error, methodName, ex.ToString());
		}

		protected Task LogCheck(LogLevel loglevel, string? methodName, string log)
		{
			if (loglevel.Level >= Level)
			{
				return Log(loglevel, methodName, log);
			}
			return Task.Run(() => { });
		}

		protected Task LogCheck(LogLevel loglevel, string? methodName, Func<string> log)
		{
			if (loglevel.Level >= Level)
			{
				return Log(loglevel, methodName, log());
			}
			return Task.Run(() => { });
		}

		/// <summary>
		/// Writes the log.
		/// </summary>
		/// <param name="loglevel">loglevel</param>
		/// <param name="methodName">name of the caller method</param>
		/// <param name="log">log text</param>
		/// <returns></returns>
		protected abstract Task Log(LogLevel loglevel, string? methodName, string log);
	}
}
