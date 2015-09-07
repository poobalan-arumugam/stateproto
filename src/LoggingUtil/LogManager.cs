using System;

namespace MurphyPA.Logging
{
	/// <summary>
	/// LogManager.
	/// </summary>
	public class LogManager
	{
	    private static ILogManager _LogManager = new ConsoleLogManager ();
	    
	    public static ILogger GetLogger(Type type)
	    {
	        return _LogManager.GetLogger (type);
	    }
	    
	    public void SetLogManager(ILogManager logManager)
	    {
	        _LogManager = logManager;
	    }
	}
}
