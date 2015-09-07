using System;

namespace qf4net
{
	/// <summary>
	/// IQEventManagerEvents.
	/// </summary>
	public interface IQEventManagerEvents
	{
        event PolledEventHandler PolledEvent;
        event EventManagerDispatchExceptionHandler EMDispatchException;
        event EventManagerDispatchCommandExceptionHandler EMDispatchCommandException;
	}
}
