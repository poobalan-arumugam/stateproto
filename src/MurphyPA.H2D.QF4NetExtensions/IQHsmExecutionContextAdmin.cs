using System;

namespace qf4net
{
	/// <summary>
	/// IQHsmExecutionContextAdmin.
	/// </summary>
	public interface IQHsmExecutionContextAdmin
	{
        void AddService (Type type, string name, object service);
        void RemoveService (Type type, string name);
        bool ContainsService (Type type, string name);
	}
}
