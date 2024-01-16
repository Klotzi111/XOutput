using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace XOutput.Logging
{
	public interface ILogger
	{
		/// <summary>
		/// Writes a trace log.
		/// </summary>
		/// <param name="log">log message</param>
		/// <returns></returns>
		Task Trace(string log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a trace log with lazy evaluation.
		/// </summary>
		/// <param name="log">log message generator</param>
		/// <returns></returns>
		Task Trace(Func<string> log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a debug log.
		/// </summary>
		/// <param name="log">log message</param>
		/// <returns></returns>
		Task Debug(string log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a debug log with lazy evaluation.
		/// </summary>
		/// <param name="log">log message generator</param>
		/// <returns></returns>
		Task Debug(Func<string> log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a info log.
		/// </summary>
		/// <param name="log">log message</param>
		/// <returns></returns>
		Task Info(string log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a info log with lazy evaluation.
		/// </summary>
		/// <param name="log">log message generator</param>
		/// <returns></returns>
		Task Info(Func<string> log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a warning log.
		/// </summary>
		/// <param name="log">log message</param>
		/// <returns></returns>
		Task Warning(string log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a warning log with lazy evaluation.
		/// </summary>
		/// <param name="log">log message generator</param>
		/// <returns></returns>
		Task Warning(Func<string> log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a warning log.
		/// </summary>
		/// <param name="ex">exception</param>
		/// <returns></returns>
		Task Warning(Exception ex, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a error log.
		/// </summary>
		/// <param name="log">log message</param>
		/// <returns></returns>
		Task Error(string log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a error log with lazy evaluation.
		/// </summary>
		/// <param name="log">log message generator</param>
		/// <returns></returns>
		Task Error(Func<string> log, [CallerMemberName] string? methodName = null);
		/// <summary>
		/// Writes a error log.
		/// </summary>
		/// <param name="ex">exception</param>
		/// <returns></returns>
		Task Error(Exception ex, [CallerMemberName] string? methodName = null);
	}
}
