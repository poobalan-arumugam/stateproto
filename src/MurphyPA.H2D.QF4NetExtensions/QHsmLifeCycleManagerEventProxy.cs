using System;
using System.Collections;
using System.Reflection;

namespace qf4net
{
	/// <summary>
	/// QHsmLifeCycleManagerEventProxy.
	/// </summary>
	public class QHsmLifeCycleManagerEventProxy : QHsmLifeCycleManagerEventsBase, IQHsmLifeCycleManager, IQHsmEvents, IQEventManagerEvents
	{
		public QHsmLifeCycleManagerEventProxy()
		{
        }

	    public void RegisterHsm(ILQHsm hsm)
	    {
            throw new NotSupportedException ("This Proxy is only for forwarding IQHsmLifeCycleManagerEvents");
	    }

	    public void UnregisterHsm(ILQHsm hsm)
	    {
            throw new NotSupportedException ("This Proxy is only for forwarding IQHsmLifeCycleManagerEvents");
        }

        public void AddLifeCycleManager (IQHsmLifeCycleManagerEvents managerEvents)
        {
            managerEvents.LifeCycleChange += new QHsmLifeCycleChangeHandler(managerEvents_LifeCycleChange);
            IQHsmEvents events = managerEvents as IQHsmEvents;
            events.StateChange += new EventHandler(events_StateChange);
            events.UnhandledTransition += new DispatchUnhandledTransitionHandler(events_UnhandledTransition);
            events.DispatchException += new DispatchExceptionHandler(events_DispatchException);
        	
			IQEventManagerEvents eventManagerEvents = managerEvents as IQEventManagerEvents;
			if(null != eventManagerEvents)
			{
				eventManagerEvents.PolledEvent += new PolledEventHandler(eventManagerEvents_PolledEvent);
				eventManagerEvents.EMDispatchException += new EventManagerDispatchExceptionHandler(eventManagerEvents_EMDispatchException);
				eventManagerEvents.EMDispatchCommandException += new EventManagerDispatchCommandExceptionHandler(eventManagerEvents_EMDispatchCommandException);
			}
        }

        private void managerEvents_LifeCycleChange(IQHsmLifeCycleManager lifeCycleManager, ILQHsm hsm, QHsmLifeCycleChangeType lifeCycleChangeType)
        {
            DoLifeCycleChange (lifeCycleManager, hsm, lifeCycleChangeType);
        }
		
        #region event StateChange
        protected virtual bool OnStateChange (object hsm, EventArgs logEvent)
        {
            return true;
        }

        protected void RaiseStateChange (EventHandler handler, object hsm, EventArgs logEvent)
        {
            if (handler != null)
            {
                handler (hsm, logEvent);
            }
        }

        protected void DoStateChange (object hsm, EventArgs logEvent)
        {
            if (OnStateChange (hsm, logEvent))
            {
                RaiseStateChange (StateChange, hsm, logEvent);
            }
        }

        public event EventHandler StateChange;
        #endregion

        #region event DispatchExceptionEvent
        public event DispatchExceptionHandler DispatchException;

        protected virtual bool OnDispatchException (IQHsm hsm, Exception ex, MethodInfo stateMethod, IQEvent ev)
        {
            return true;
        }

        protected void RaiseDispatchException (DispatchExceptionHandler handler, IQHsm hsm, Exception ex, MethodInfo stateMethod, IQEvent ev)
        {
            if (handler != null)
            {
                handler (ex, hsm, stateMethod, ev);
            }
        }

        protected virtual void DoDispatchException (IQHsm hsm, Exception ex, MethodInfo stateMethod, IQEvent ev)
        {
            if (OnDispatchException (hsm, ex, stateMethod, ev))
            {
                RaiseDispatchException (DispatchException, hsm, ex, stateMethod, ev);
            }			
        }
        #endregion

        #region event UnhandledTransition
        public event DispatchUnhandledTransitionHandler UnhandledTransition;

        protected virtual bool OnUnhandledTransition (IQHsm hsm, MethodInfo stateMethod, IQEvent qEvent)
        {
            return true;
        }

        protected void RaiseUnhandledTransition (DispatchUnhandledTransitionHandler handler, IQHsm hsm, MethodInfo stateMethod, IQEvent qEvent)
        {
            if (handler != null)
            {
                handler (hsm, stateMethod, qEvent);
            }
        }

        protected void DoUnhandledTransition (IQHsm hsm, MethodInfo stateMethod, IQEvent qEvent)
        {
            if (OnUnhandledTransition (hsm, stateMethod, qEvent))
            {
                RaiseUnhandledTransition (UnhandledTransition, hsm, stateMethod, qEvent);
            }
        }
        #endregion
		
		#region event PolledEvent
		public event PolledEventHandler PolledEvent;

		protected virtual bool OnPolledEvent (IQEventManager eventManager, IQHsm hsm, IQEvent ev, PollContext pollContext)
		{
			return true;
		}

		protected void RaisePolledEvent (IQEventManager eventManager, PolledEventHandler handler, IQHsm hsm, IQEvent ev, PollContext pollContext)
		{
			if (handler != null)
			{
				handler (eventManager, hsm, ev, pollContext);
			}
		}

		protected void DoPolledEvent (IQEventManager eventManager, IQHsm hsm, IQEvent ev, PollContext pollContext)
		{
			if (OnPolledEvent (eventManager, hsm, ev, pollContext))
			{
				RaisePolledEvent (eventManager, PolledEvent, hsm, ev, pollContext);
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

        private void events_StateChange(object sender, EventArgs e)
        {
            DoStateChange (sender, e);
        }

        private void events_UnhandledTransition(IQHsm hsm, MethodInfo stateMethod, IQEvent ev)
        {
            DoUnhandledTransition (hsm, stateMethod, ev);
        }

        private void events_DispatchException(Exception ex, IQHsm hsm, MethodInfo stateMethod, IQEvent ev)
        {
            DoDispatchException (hsm, ex, stateMethod, ev);
		}

		private void eventManagerEvents_PolledEvent(IQEventManager eventManager, IQHsm hsm, IQEvent ev, PollContext pollContext)
		{
			DoPolledEvent(eventManager, hsm, ev, pollContext);
		}

		private void eventManagerEvents_EMDispatchException(IQEventManager eventManager, Exception ex, IQHsm hsm, IQEvent ev)
		{
			DoEventManagerDispatchException(eventManager, ex, hsm, ev);
		}

		private void eventManagerEvents_EMDispatchCommandException(IQEventManager eventManager, Exception ex, IQSimpleCommand command)
		{
			DoEventManagerDispatchCommandException(eventManager, ex, command);
		}
	}
}
