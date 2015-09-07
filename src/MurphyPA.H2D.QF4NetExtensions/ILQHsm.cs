using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for ILQHsm.
	/// </summary>
	public interface ILQHsm : IQHsm
	{
		string Id { get; }
        string GroupId { get; }
        string UnderlyingObjectId { get; }

		//void Init ();

		IQEventManager EventManager { get; }

        bool IsFinalState (QState state);

		//void Dispatch (IQEvent ev);
		void AsyncDispatch (IQEvent ev);
		void AsyncDispatchFront (IQEvent ev);

		void SetTimeOut (string name, TimeSpan duration, IQEvent ev);
		void SetTimeOut (string name, TimeSpan duration, IQEvent ev, TimeOutType timeOutType);
		void SetTimeOut (string name, DateTime at, IQEvent ev);
		void SetTimeOut (string name, DateTime at, IQEvent ev, TimeOutType timeOutType);
		void ClearTimeOut (string name);

		event EventHandler StateChange;
        event EventHandler FinalStateReached;

		ModelInformation ModelInformation { get; }

		TransitionEventAttribute[] TransitionEvents { get; }

        void RegisterWithLifeCycleManager (IQHsmLifeCycleManager lifeCycleManager);
        void RegisterWithExecutionContext (IQHsmExecutionContext executionContext);

		// non standard serialisation - did not want to be forced to use ISerialisable
		void Serialise (ISerialisationContext context);
		void Deserialise (ISerialisationContext context);

		void SaveToMemento (ILQHsmMemento memento);
		void RestoreFromMemento (ILQHsmMemento memento);
	}
}
