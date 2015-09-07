using System;
using qf4net;

namespace RunToCompletionHsm
{
	public class RtcEvent : QEvent 
	{
		public RtcEvent(RtcSignals signal):base((int)signal)
		{
		}//ctor

	}//RtcEvent


}//namespace RunToCompletionHsm
