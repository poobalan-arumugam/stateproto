using System;
using qf4net;

namespace DiningPhilosophers
{
	/// <summary>
	/// Summary description for PhilosopherEvent.
	/// </summary>
	public class PhilosopherEvent : QEvent
	{
		internal PhilosopherEvent(DPPSignal signal):base((int)signal)
		{
		}

		/// <summary>
		/// The QSignal in string form. It allows for simpler debugging and logging. 
		/// </summary>
		/// <returns>The signal as string.</returns>
		public override string ToString()
		{
			switch (this.QSignal)
			{
				case (int)DPPSignal.Done:
				case (int)DPPSignal.Hungry:
				case (int)DPPSignal.Timeout:
					return ((DPPSignal)this.QSignal).ToString();
				default: return base.ToString();
			}
		}
	}
}
