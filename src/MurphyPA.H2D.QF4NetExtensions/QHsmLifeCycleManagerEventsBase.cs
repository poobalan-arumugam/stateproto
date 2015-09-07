using System;
using System.Collections;

namespace qf4net
{
	/// <summary>
	/// QHsmLifeCycleManagerEventsBase.
	/// </summary>
	public class QHsmLifeCycleManagerEventsBase : LoggingUserBase, IQHsmLifeCycleManagerEvents
	{
		public QHsmLifeCycleManagerEventsBase()
		{
        }

        #region event LifeCycleChange
        protected void RaiseLifeCycleChange (QHsmLifeCycleChangeHandler handler, IQHsmLifeCycleManager lifeCycleManager, ILQHsm hsm, QHsmLifeCycleChangeType lifeCycleChangeType)
        {
            try 
            {
                if (handler != null)
                {
                    handler (lifeCycleManager, hsm, lifeCycleChangeType);
                }
            } catch (Exception ex)
            {
                Logger.Error (ex, "LifeCycleChange event handler raised an exception.");
            }
        }

        protected virtual bool OnLifeCycleChange (IQHsmLifeCycleManager lifeCycleManager, ILQHsm hsm, QHsmLifeCycleChangeType lifeCycleChangeType)
        {
            return true;
        }

        protected void DoLifeCycleChange (IQHsmLifeCycleManager lifeCycleManager, ILQHsm hsm, QHsmLifeCycleChangeType lifeCycleChangeType)
        {
            if (OnLifeCycleChange (lifeCycleManager, hsm, lifeCycleChangeType))
            {
                RaiseLifeCycleChange (LifeCycleChange, lifeCycleManager, hsm, lifeCycleChangeType);
            }
        }

        public event QHsmLifeCycleChangeHandler LifeCycleChange;
        #endregion

	}
}
