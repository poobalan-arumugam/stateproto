using System;
using qf4net;

namespace RunToCompletionHsm
{

	public enum RtcSignals : int
	{
		Start =	QSignals.UserSig, //enum values must start at UserSig value or greater
		Abort,
		Quit

	}//RtcSignals

}//namespace RunToCompletionHsm
