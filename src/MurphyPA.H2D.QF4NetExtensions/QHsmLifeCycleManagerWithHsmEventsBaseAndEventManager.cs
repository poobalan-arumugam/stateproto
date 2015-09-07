using System;

namespace qf4net
{
	/// <summary>
	/// QHsmLifeCycleManagerWithHsmEventsBaseAndEventManager.
	/// </summary>
	public class QHsmLifeCycleManagerWithHsmEventsBaseAndEventManager : QHsmLifeCycleManagerWithHsmEventsBaseAndEventManagerBase
	{
        public QHsmLifeCycleManagerWithHsmEventsBaseAndEventManager(IQEventManager eventManager)
        {
            System.Diagnostics.Debug.Assert (eventManager != null);
            _EventManager = eventManager;
            RegisterEventManager (eventManager);
        }

        IQEventManager _EventManager;

        protected override IQEventManager GetEventManager(ILQHsm hsm)
        {
            return _EventManager;
        }
    }
}
