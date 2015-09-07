using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for IStateChangeListener.
	/// </summary>
	public interface IQStateChangeListener
	{
		void HandleStateChange (object sender, EventArgs e);
	}
}
