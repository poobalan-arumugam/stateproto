using System;

namespace qf4net
{
	/// <summary>
	/// IQHsmLifeCycleManagerEvents.
	/// </summary>
	public interface IQHsmLifeCycleManagerEvents
	{
        event QHsmLifeCycleChangeHandler LifeCycleChange;
	}
}
