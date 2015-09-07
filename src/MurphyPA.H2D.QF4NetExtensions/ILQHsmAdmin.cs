using System;

namespace qf4net
{
	/// <summary>
	/// ILQHsmAdmin.
	/// </summary>
	public interface ILQHsmAdmin
	{
        IQEventManager EventManager { get; set; }
	}
}
