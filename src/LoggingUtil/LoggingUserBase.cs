using System;
using MurphyPA.Logging;

namespace System
{
	/// <summary>
	/// LoggingUserBase.
	/// </summary>
	public class LoggingUserBase
	{
	    private ILogger _Logger;
	    public ILogger Logger
	    {
	        get
	        {
	            if(null == _Logger)
	            {
	                lock(this)
	                {
                        if(null == _Logger)
                        {
                            _Logger = MurphyPA.Logging.LogManager.GetLogger (this.GetType ());
                        }
	                }
	            }
	            return _Logger;
	        }
	    }
	}
}
