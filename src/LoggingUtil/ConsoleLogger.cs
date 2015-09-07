using System;

namespace MurphyPA.Logging
{
	/// <summary>
	/// ConsoleLogger.
	/// </summary>
	public class ConsoleLogger : ILogger
	{
	    public ConsoleLogger(string owner)
	    {
	        _Owner = owner;
	    }
	    
	    private string _Owner;
	    
	    private void Log(string scope, string fmt, params object[] args)
	    {
	        string msg;
	        if(null == args || args.Length == 0)
	        {	            
	            msg = fmt;
	        }else
	        {
                msg = string.Format(fmt, args);
	        }
	        string dateTime = DateTime.Now.ToString ("yyyy-MM-dd hh:mm:ss.fff");
	        string threadName = System.Threading.Thread.CurrentThread.Name;
	        msg = string.Format ("{0} {1} [{2}] {3} {4}", dateTime, scope, threadName, _Owner, msg);
	        Console.WriteLine (msg);
	    }
	    
        #region ILogger Members

        public void Debug(string fmt, params object[] args)
        {
            Log ("DEBUG", fmt, args);
        }

        public void Info(string fmt, params object[] args)
        {
            Log ("INFO", fmt, args);
        }

        public void Warn(string fmt, params object[] args)
        {
            Log ("WARN", fmt, args);
        }

        public void Error(string fmt, params object[] args)
        {
            Log ("ERROR", fmt, args);
        }

        void MurphyPA.Logging.ILogger.Error(Exception ex, string fmt, params object[] args)
        {
            string errorMsg = string.Format ("ERROR {0}", ex);
            Log (errorMsg, fmt, args);
        }

        #endregion
    }
}
