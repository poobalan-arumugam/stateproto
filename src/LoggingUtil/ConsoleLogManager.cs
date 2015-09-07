using System;

namespace MurphyPA.Logging
{
	/// <summary>
	/// Summary description for ConsoleLogManager.
	/// </summary>
	public class ConsoleLogManager : ILogManager
	{
        #region ILogManager Members

        public ILogger GetLogger(Type type)
        {
            return new ConsoleLogger (type.ToString ());
        }

        #endregion
    }
}
