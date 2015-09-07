using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for IQTimer.
	/// </summary>
	public interface IQTimer
	{
		void SetTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev);
		void SetTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev, TimeOutType timeOutType);
		void SetTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev);
		void SetTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev, TimeOutType timeOutType);
		void ClearTimeOut (string name);

		event QTimeoutHandler TimeOut;
	}

	public delegate void QTimeoutHandler (IQTimer timer, IQHsm hsm, IQEvent ev);
}
