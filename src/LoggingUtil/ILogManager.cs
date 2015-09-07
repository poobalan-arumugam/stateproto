using System;

namespace MurphyPA.Logging
{
	/// <summary>
	/// ILogManager.
	/// </summary>
	public interface ILogManager
	{
	    ILogger GetLogger(Type type);
	}
}
