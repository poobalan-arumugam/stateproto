using System;

namespace qf4net
{
	/// <summary>
	/// IQHsmExecutionContext.
	/// </summary>
	public interface IQHsmExecutionContext
	{
        IQHsmLifeCycleManager LifeCycleManager { get; }
        object GetService (Type type, string name);
	}
}
