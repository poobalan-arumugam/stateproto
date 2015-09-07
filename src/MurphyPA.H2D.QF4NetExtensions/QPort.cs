using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for QPort.
	/// </summary>
	public class QPort : IQPort
	{
		LQHsm _Qhsm;

		public QPort (string name, LQHsm qhsm)
		{
			_Name = name;
			_Qhsm = qhsm;
		}

        public QPort (string name, string key, LQHsm qhsm)
        {
            _Name = name;
            _Key = key;
            _Qhsm = qhsm;
        }

		#region IQPort Members

		string _Name;
		public string Name { get { return _Name; } }

        string _Key;
        public string Key { get { return _Key; } }

		public void Receive (IQPort fromPort, IQEvent ev)
		{
			ev = new QEvent (_Name, _Key, ev.QSignal, ev.QData, ev.QSent);
			_Qhsm.AsyncDispatch (ev);
		}

		protected virtual bool OnQEvents (IQEvent ev)
		{
			return true;
		}

		public void Send(IQEvent ev)
		{
			if (OnQEvents (ev))
			{
				if (QEvents != null)
				{
					QEvents (this, ev);
				}
			}
		}

		public event qf4net.QEventHandler QEvents;

		#endregion
	}
}
