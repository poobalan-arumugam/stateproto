using System;

namespace qf4net
{
	/// <summary>
	/// IQFTransaction.
	/// </summary>
	public interface IQFTransaction
	{
        void Commit ();
        void Abort ();
	}
}
