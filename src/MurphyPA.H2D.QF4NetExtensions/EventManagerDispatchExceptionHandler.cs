using System;

namespace qf4net
{
	/// <summary>
	/// EventManagerDispatchExceptionHandler.
	/// </summary>
    public delegate void EventManagerDispatchExceptionHandler (IQEventManager eventManager, Exception ex, IQHsm hsm, IQEvent ev);
}
