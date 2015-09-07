using System;
using qf4net;

namespace OrthogonalComponentHsm
{

	public enum AlarmClockSignals : int
	{
		Time =	QSignals.UserSig, //enum values must start at UserSig value or greater
		Start,//used in place of automatic Init signal for this example (departs from C++ example)
		Alarm,
		AlarmOn,
		AlarmOff,
		Mode12Hour,
		Mode24Hour,
		Terminate

	}//AlarmClockSignals

}//namespace OrthogonalComponentHsm
