using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for QMultiPort.
	/// </summary>
	public class QMultiPort : IQMultiPort
	{
		LQHsm _Qhsm;

		public QMultiPort(string name, LQHsm qhsm)
		{
			_Qhsm = qhsm;
			_Name = name;
		}

		#region IQMultiPort Members

		string _Name;
		public string Name { get { return _Name; } }

		System.Collections.Hashtable _Ports = new System.Collections.Hashtable ();

		public bool HasPort (string key)
		{
			lock (_Ports.SyncRoot)
			{
				bool hasPort = _Ports.Contains (key);
				return hasPort;
			}
		}

		public void CreatePort(string key)
		{
			lock (_Ports.SyncRoot)
			{
				if (!_Ports.Contains (key))
				{
                    IQPort port = new QPort (_Name, key, _Qhsm);
					_Ports.Add (key, port);
                    DoPortCreated (key, port);
                }
			}
		}

        #region event PortRequested
        public event PortCreatedHandler PortCreated;

        protected void RaisePortRequested (PortCreatedHandler handler, string key, IQPort port)
        {
            if (handler != null)
            {
                handler (this, key, port);
            }
        }

        protected virtual bool OnPortCreated (string key, IQPort port)
        {
            return true;
        }

        protected void DoPortCreated (string key, IQPort port)
        {
            if (OnPortCreated (key, port))
            {
                RaisePortRequested (PortCreated, key, port);
            }
        }
        #endregion

		public IQPort this[string key]
		{
			get
			{
				lock (_Ports.SyncRoot)
				{
					IQPort port = _Ports[key] as IQPort;
                    if (null == port)
                    {
                        CreatePort (key);
                        port = _Ports[key] as IQPort;
                    }
					return port;
				}
			}
		}

        object _Arg;
        public object Arg { get { return _Arg; } set { _Arg = value; } }

		#endregion
	}
}
