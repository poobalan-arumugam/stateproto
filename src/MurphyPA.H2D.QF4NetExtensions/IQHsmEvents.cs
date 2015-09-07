using System;

namespace qf4net
{
	/// <summary>
	/// IQHsmEvents.
	/// </summary>
	public interface IQHsmEvents
	{
        event EventHandler StateChange;
        event DispatchExceptionHandler DispatchException; 
        event DispatchUnhandledTransitionHandler UnhandledTransition;
	}
}
