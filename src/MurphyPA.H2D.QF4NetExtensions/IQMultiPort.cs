using System;

namespace qf4net
{
	/// <summary>
	/// IQMultiPort.
	/// </summary>
	public interface IQMultiPort
	{
		string Name { get; }
		bool HasPort (string key);
		void CreatePort (string key);
		IQPort this [string key] { get; }
        object Arg { get; set; }

        event PortCreatedHandler PortCreated;
	}

    public delegate void PortCreatedHandler (IQMultiPort mport, string key, IQPort port);
}
