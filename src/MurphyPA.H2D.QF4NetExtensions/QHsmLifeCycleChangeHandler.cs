using System;

namespace qf4net
{
	/// <summary>
	/// QHsmLifeCycleChangeHandler.
	/// </summary>
    public delegate void QHsmLifeCycleChangeHandler (IQHsmLifeCycleManager lifeCycleManager, ILQHsm hsm, QHsmLifeCycleChangeType lifeCycleChangeType);
}
