using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for QStateChangeListenerBase.
	/// </summary>
	public class QStateChangeListenerBase : LoggingUserBase, IQStateChangeListener
	{
		IQStateChangeListener _Listener;

		public QStateChangeListenerBase(IQStateChangeListener listener)
		{
			_Listener = listener;
		}

		#region IQStateChangeListener Members

		public void HandleStateChange(object sender, EventArgs e)
		{
			if (_Listener != null)
			{
				_Listener.HandleStateChange (sender, e);
			}
		}

		#endregion
	}
}
