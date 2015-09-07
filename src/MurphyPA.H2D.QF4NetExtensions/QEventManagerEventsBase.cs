using System;

namespace qf4net
{
    /// <summary>
    /// QEventManager.
    /// </summary>
    public abstract class QEventManagerEventsBase : LoggingUserBase, IQEventManagerEvents
    {
        public QEventManagerEventsBase ()
        {
        }

        #region event PolledEvent
        public event PolledEventHandler PolledEvent;

        protected virtual bool OnPolledEvent (IQEventManager eventManager, HsmEventHolder holder, PollContext pollContext)
        {
            return true;
        }

        protected void RaisePolledEvent (IQEventManager eventManager, PolledEventHandler handler, HsmEventHolder holder, PollContext pollContext)
        {
            if (handler != null)
            {
                handler (eventManager, holder.Hsm, holder.Event, pollContext);
            }
        }

		protected void DoPolledEvent (IQEventManager eventManager, HsmEventHolder holder, PollContext pollContext)
		{
			if (OnPolledEvent (eventManager, holder, pollContext))
			{
                RaisePolledEvent (eventManager, PolledEvent, holder, pollContext);
			}
		}
        #endregion

        #region event EMDispatchException
		protected virtual bool OnEventManagerDispatchException (IQEventManager eventManager, Exception ex, IQHsm hsm, IQEvent ev)
		{
			return true;
		}

        protected void RaiseEventManagerDispatchException (EventManagerDispatchExceptionHandler handler, IQEventManager eventManager, Exception ex, IQHsm hsm, IQEvent ev)
        {
            if (handler != null)
            {
                handler (eventManager, ex, hsm, ev);
            }
        }

		protected void DoEventManagerDispatchException (IQEventManager eventManager, Exception ex, IQHsm hsm, IQEvent ev)
		{
            if (OnEventManagerDispatchException (eventManager, ex, hsm, ev))
            {
                RaiseEventManagerDispatchException (EMDispatchException, eventManager, ex, hsm, ev);
            }
		}

		public event EventManagerDispatchExceptionHandler EMDispatchException;

		#endregion

        #region event EMDispatchCommandException
        protected virtual bool OnEventManagerDispatchCommandException (IQEventManager eventManager, Exception ex, IQSimpleCommand command)
        {
            return true;
        }

        protected void RaiseEventManagerDispatchCommandException (EventManagerDispatchCommandExceptionHandler handler, IQEventManager eventManager, Exception ex, IQSimpleCommand command)
        {
            if (handler != null)
            {
                handler (eventManager, ex, command);
            }
        }

        protected void DoEventManagerDispatchCommandException (IQEventManager eventManager, Exception ex, IQSimpleCommand command)
        {
            if (OnEventManagerDispatchCommandException (eventManager, ex, command))
            {
                RaiseEventManagerDispatchCommandException (EMDispatchCommandException, eventManager, ex, command);
            }
        }

        public event EventManagerDispatchCommandExceptionHandler EMDispatchCommandException;

        #endregion
	}
}
