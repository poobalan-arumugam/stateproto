using System;

namespace qf4net
{
	/// <summary>
	/// IQEventManager.
	/// </summary>
	public interface IQEventManager : IQEventManagerEvents
	{
		void AsyncDispatchFront (IQHsm hsm, IQEvent ev);
		void AsyncDispatch (IQHsm hsm, IQEvent ev);
		void Dispatch (IQHsm hsm, IQEvent ev);
        void AsyncDispatch (IQSimpleCommand cmd);
		bool PollOne ();
		bool Poll ();
		bool WaitOne (TimeSpan duration);

		void SetTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev);
		void SetTimeOut (string name, TimeSpan duration, IQHsm hsm, IQEvent ev, TimeOutType timeOutType);
		void SetTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev);
		void SetTimeOut (string name, DateTime at, IQHsm hsm, IQEvent ev, TimeOutType timeOutType);
		void ClearTimeOut (string name);

		IQEventManagerRunner Runner { get; set; } // optional auto runner interface
	}

	public enum PollContext {BeforeHandled, AfterHandled}

	public enum TimeOutType {Single, Repeat}

	public delegate void PolledEventHandler (IQEventManager eventManager, IQHsm hsm, IQEvent ev, PollContext pollContext);
}
