using System;
using qf4net;

namespace DiningPhilosophers
{
	/// <summary>
	/// Summary description for TableEvent.
	/// </summary>
	public class TableEvent : QEvent
	{
		internal readonly int PhilosopherId;

		internal TableEvent(DPPSignal signal, int philosopherId):base((int)signal)
		{
			PhilosopherId = philosopherId;
		}

		/// <summary>
		/// The QSignal in string form. It allows for simpler debugging and logging. 
		/// </summary>
		/// <returns>The signal as string.</returns>
		public override string ToString()
		{
			switch (this.QSignal)
			{
				case (int)DPPSignal.Hungry:
				case (int)DPPSignal.Eat:
				case (int)DPPSignal.Done:
					return String.Format("Signal {0}; Philosopher {1}", ((DPPSignal)this.QSignal).ToString(), PhilosopherId);
				default: return base.ToString();
			}
		}
	}
}
