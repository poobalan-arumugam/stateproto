using System;

namespace qf4net
{
	/// <summary>
	/// QHsmLifeCycleManagerWithHsmEventsBaseAndMultipleEventManagers.
	/// </summary>
	public class QHsmLifeCycleManagerWithHsmEventsBaseAndMultipleEventManagers : QHsmLifeCycleManagerWithHsmEventsBaseAndEventManagerBase
	{
		public QHsmLifeCycleManagerWithHsmEventsBaseAndMultipleEventManagers(IQEventManager[] eventManagers)
		{
            System.Diagnostics.Debug.Assert (eventManagers != null);
            System.Diagnostics.Debug.Assert (eventManagers.Length > 0);
            _EventManagers = eventManagers;
            foreach (IQEventManager eventManager in eventManagers)
            {
                RegisterEventManager (eventManager);
            }
		}

        IQEventManager[] _EventManagers;

        private int GetHashCode (string name)
        {
            return name.GetHashCode ();
        }

        protected override IQEventManager GetEventManager(ILQHsm hsm)
        {
            string name = string.Format ("{0}", hsm.GroupId);
            int hashCode = GetHashCode (name);
            int index = hashCode % _EventManagers.Length;
            Logger.Debug ("HashCode returned for: {0} is {1} results in index: {2}", name, hashCode, index);
            index = Math.Abs (index);
            return _EventManagers [index];
        }
    }
}
