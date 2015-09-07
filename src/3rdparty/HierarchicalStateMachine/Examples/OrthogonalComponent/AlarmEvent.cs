using System;
using qf4net;

namespace OrthogonalComponentHsm
{
	public class AlarmInitEvent : QEvent 
	{
		public AlarmInitEvent(AlarmClockSignals signal):base((int)signal)
		{
		}//ctor

	}//AlarmInitEvent

	public class TimeEvent : QEvent 
	{
		public DateTime CurrentTime
		{
			get { return currentTime; }
			set { currentTime = value; }
		}
		private DateTime currentTime;

		public TimeEvent(DateTime currentTime, AlarmClockSignals signal):base((int)signal)
		{
			this.currentTime = currentTime;
		}//ctor

	}//TimeEvent

}//namespace OrthogonalComponentHsm
