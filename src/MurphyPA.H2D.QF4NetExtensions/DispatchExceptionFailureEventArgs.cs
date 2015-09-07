using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for DispatchExceptionFailureEventArgs.
	/// </summary>
	[Serializable]
	public class DispatchExceptionFailureEventArgs
	{
		public DispatchExceptionFailureEventArgs(Exception ex, IQHsm hsm, System.Reflection.MethodInfo stateMethod, IQEvent ev)
		{
			_Exception = ex;
			_Hsm = hsm;
			_StateMethod = stateMethod;
			_OriginalEvent = ev;
		}

		Exception _Exception;
		public Exception Exception { get { return _Exception; } }

		IQHsm _Hsm;
		public IQHsm Hsm { get { return _Hsm; } }

		System.Reflection.MethodInfo _StateMethod;
		public System.Reflection.MethodInfo StateMethod { get { return _StateMethod; } }

		IQEvent _OriginalEvent;
		public IQEvent OriginalEvent { get { return _OriginalEvent; } }
	}
}
