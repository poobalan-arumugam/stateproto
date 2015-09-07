using System;
using System.Reflection;

namespace qf4net
{
	/// <summary>
	/// QHsmLifeCycleManagerWithHsmEventsBase.
	/// </summary>
	public class QHsmLifeCycleManagerWithHsmEventsBase : QHsmLifeCycleManagerBase, IQHsmEvents
	{
		public QHsmLifeCycleManagerWithHsmEventsBase()
		{
        }

        protected override bool OnLifeCycleChange(IQHsmLifeCycleManager lifeCycleManager, ILQHsm hsm, QHsmLifeCycleChangeType lifeCycleChangeType)
        {
            switch (lifeCycleChangeType)
            {
                case QHsmLifeCycleChangeType.Added:
                {
                    RegisterEvents (hsm);
                } break;
                case QHsmLifeCycleChangeType.Removed:
                {
                    UnRegisterEvents (hsm);
                } break;
                default: break; // not really interested in any other events.
            }
            return base.OnLifeCycleChange (lifeCycleManager, hsm, lifeCycleChangeType);
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


	    protected virtual void RegisterEvents(ILQHsm hsm)
	    {
            hsm.StateChange += new EventHandler(hsm_StateChange);
            hsm.UnhandledTransition += new DispatchUnhandledTransitionHandler(hsm_UnhandledTransition);
	        hsm.DispatchException += new DispatchExceptionHandler(hsm_DispatchException);
	    }

	    protected virtual void UnRegisterEvents(ILQHsm hsm)
	    {
            hsm.StateChange -= new EventHandler(hsm_StateChange);
            hsm.UnhandledTransition -= new DispatchUnhandledTransitionHandler(hsm_UnhandledTransition);
            hsm.DispatchException -= new DispatchExceptionHandler(hsm_DispatchException);	        
        }

        private void hsm_StateChange(object sender, EventArgs e)
        {
            DoStateChange (sender, e);
        }

        private void hsm_UnhandledTransition(IQHsm hsm, System.Reflection.MethodInfo stateMethod, IQEvent ev)
        {
            DoUnhandledTransition (hsm, stateMethod, ev);
        }

        private void hsm_DispatchException(Exception ex, IQHsm hsm, System.Reflection.MethodInfo stateMethod, IQEvent ev)
        {
            DoDispatchException(hsm, ex, stateMethod, ev);
        }
    }
}
