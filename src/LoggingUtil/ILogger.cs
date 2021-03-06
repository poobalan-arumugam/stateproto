using System;

namespace MurphyPA.Logging
{
	/// <summary>
	/// ILogger - wrapping around Log4Net.
	/// </summary>
	public interface ILogger
	{
	    void Debug(string fmt, params object[] args);
        void Info(string fmt, params object[] args);
        void Warn(string fmt, params object[] args);
        void Error(string fmt, params object[] args);
        void Error(Exception ex, string fmt, params object[] args);
    }
}
