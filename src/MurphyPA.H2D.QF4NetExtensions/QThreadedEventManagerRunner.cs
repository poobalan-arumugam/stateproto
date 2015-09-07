using System;
using System.Threading;

namespace qf4net
{
	/// <summary>
	/// Summary description for QThreadedEventManagerRunner.
	/// </summary>
	public class QThreadedEventManagerRunner : IQEventManagerRunner
	{
		IQEventManager _EventManager;
		Thread _Thread;
        string _Name;

		public QThreadedEventManagerRunner (string name, IQEventManager eventManager)
		{
            _Name = name;
			_EventManager = eventManager;
			_EventManager.Runner = this;
		}

        public QThreadedEventManagerRunner (IQEventManager eventManager)
            : this (null, eventManager)
        {
        }

		bool _Quit = false;
		public void Run ()
		{
			while (!_Quit)
			{
				_EventManager.WaitOne (TimeSpan.FromSeconds (1));
				_EventManager.Poll ();
			}
		}

		public void Stop ()
		{
			_Quit = true;
			_Thread.Join ();
		}

		public void Start ()
		{
			Thread thr = new Thread (new ThreadStart (Run));
			thr.IsBackground = true;
            if (_Name != null)
            {
                thr.Name = _Name;
            }
			thr.Start ();
			_Thread = thr;
		}
	}
}
