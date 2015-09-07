using System;

namespace qf4net
{
    /// <summary>
    /// EventManagerDispatchCommandExceptionHandler.
    /// </summary>
    public delegate void EventManagerDispatchCommandExceptionHandler (IQEventManager eventManager, Exception ex, IQSimpleCommand command);
}
