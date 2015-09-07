using System;

namespace qf4net
{
	/// <summary>
	/// IQHsmLifeCycleManager.
	/// </summary>
	public interface IQHsmLifeCycleManager : IQHsmLifeCycleManagerEvents
	{
        void RegisterHsm (ILQHsm hsm);
        void UnregisterHsm (ILQHsm hsm);
	}
}
