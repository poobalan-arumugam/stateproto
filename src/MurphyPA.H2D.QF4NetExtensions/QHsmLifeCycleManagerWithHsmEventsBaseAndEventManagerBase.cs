using System;

namespace qf4net
{
	/// <summary>
	/// QHsmLifeCycleManagerWithHsmEventsBaseAndEventManagerBase.
	/// </summary>
	public abstract class QHsmLifeCycleManagerWithHsmEventsBaseAndEventManagerBase : QHsmLifeCycleManagerWithHsmEventsBase, IQEventManagerEvents
	{
		public QHsmLifeCycleManagerWithHsmEventsBaseAndEventManagerBase()
		{
		}

        protected abstract IQEventManager GetEventManager (ILQHsm hsm);

        protected void RegisterEventManager (IQEventManager eventManager)
        {
            eventManager.PolledEvent += new PolledEventHandler(_EventManager_PolledEvent);
            eventManager.EMDispatchException += new EventManagerDispatchExceptionHandler (_EventManager_EMDispatchException);            
            eventManager.EMDispatchCommandException += new EventManagerDispatchCommandExceptionHandler(_EventManager_EMDispatchCommandException);
        }

        protected void UnregisterEventManager (IQEventManager eventManager)
        {
            eventManager.PolledEvent -= new PolledEventHandler(_EventManager_PolledEvent);
            eventManager.EMDispatchException -= new EventManagerDispatchExceptionHandler (_EventManager_EMDispatchException);            
            eventManager.EMDispatchCommandException -= new EventManagerDispatchCommandExceptionHandler(_EventManager_EMDispatchCommandException);
        }

        protected override bool OnLifeCycleChange(IQHsmLifeCycleManager lifeCycleManager, ILQHsm hsm, QHsmLifeCycleChangeType lifeCycleChangeType)
        {
            if (lifeCycleChangeType == QHsmLifeCycleChangeType.Added)
            {
                ILQHsmAdmin admin = hsm as ILQHsmAdmin;
                if (admin == null)
                {
                    throw new InvalidOperationException ("Hsm must support ILQHsmAdmin interface");
                }
                IQEventManager eventManager = GetEventManager (hsm);
                admin.EventManager = eventManager;
            }
            return base.OnLifeCycleChange (lifeCycleManager, hsm, lifeCycleChangeType);
        }


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

        private void _EventManager_PolledEvent(IQEventManager eventManager, IQHsm hsm, IQEvent ev, PollContext pollContext)
        {
            DoPolledEvent (eventManager, hsm, ev, pollContext);
        }

        private void _EventManager_EMDispatchException(IQEventManager eventManager, Exception ex, IQHsm hsm, IQEvent ev)
        {
            DoEventManagerDispatchException (eventManager, ex, hsm, ev);
        }

        private void _EventManager_EMDispatchCommandException(IQEventManager eventManager, Exception ex, IQSimpleCommand command)
        {
            DoEventManagerDispatchCommandException (eventManager, ex, command);
        }
    }
}
