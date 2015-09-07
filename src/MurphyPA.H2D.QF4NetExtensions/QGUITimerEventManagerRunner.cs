using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for GUITimerEventManagerRunner.
	/// </summary>
	public class QGUITimerEventManagerRunner : IQEventManagerRunner
	{
		IQEventManager _EventManager;
		System.Windows.Forms.Timer _Timer;

		public QGUITimerEventManagerRunner(IQEventManager eventManager, int interval)
		{
			_EventManager = eventManager;
			_EventManager.Runner = this;
			_Timer = new System.Windows.Forms.Timer ();
			_Timer.Interval = interval;
			_Timer.Tick += new EventHandler(_Timer_Tick);
		}

		bool _Quit = false;
		protected void Run ()
		{
			if (!_Quit) 
			{
				_EventManager.PollOne ();
			}
		}

		#region IQEventManagerRunner Members

		public void Start()
		{
			_Quit = false;
			_Timer.Enabled = true;
		}

		public void Stop()
		{
			_Quit = true;
			_Timer.Enabled = false;
		}

		#endregion

		private void _Timer_Tick(object sender, EventArgs e)
		{
			Run ();
		}
	}
}
