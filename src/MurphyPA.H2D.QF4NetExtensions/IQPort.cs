using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for IQPort.
	/// </summary>
	public interface IQPort
	{
		string Name { get; }
		void Send (IQEvent ev);
		event QEventHandler QEvents;

		void Receive (IQPort fromPort, IQEvent ev);
	}

	public delegate void QEventHandler (IQPort port, IQEvent ev);
}
