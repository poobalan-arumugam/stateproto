using System;
using qf4net;

namespace DiningPhilosophers
{
	public enum DPPSignal : int
	{
		Hungry = QSignals.UserSig,	// Sent by philosopher when becoming hungry
		Done,						// Sent by philosopher when done eating
		Eat,						// Sent by table to let philosopher eat 
		Timeout,					// Timeout to end thinking or eating
		MaxSignal					// Keep this signal always last
	};
}
