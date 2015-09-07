using System;
using System.Collections;

namespace qf4net
{
	/// <summary>
	/// QHsmExecutionContext.
	/// </summary>
	public class QHsmExecutionContext : IQHsmExecutionContext, IQHsmExecutionContextAdmin
	{
		public QHsmExecutionContext(IQHsmLifeCycleManager lifeCycleManager)
		{
            _LifeCycleManager = lifeCycleManager;
		}

        IQHsmLifeCycleManager _LifeCycleManager;
        public IQHsmLifeCycleManager LifeCycleManager { get { return _LifeCycleManager; } }

        Hashtable _Services = new Hashtable ();

        protected string GetKeyForService(Type type, string name)
        {
            string key = string.Format ("{0}-{1}", type, name);   
            return key;
        }

        public void AddService(Type type, string name, object service)
        {
            string key = GetKeyForService (type, name);
            lock (_Services.SyncRoot)
            {
                _Services.Add (key, service);
            }            
        }

        public void RemoveService(Type type, string name)
        {
            string key = GetKeyForService (type, name);
            lock (_Services.SyncRoot)
            {
                _Services.Remove (key);
            }            
        }

        public bool ContainsService (Type type, string name)
        {
            string key = GetKeyForService (type, name);
            lock (_Services.SyncRoot)
            {
                return _Services.Contains (key);
            }            
        }

	    public object GetService(Type type, string name)
	    {
            string key = GetKeyForService (type, name);
            lock (_Services.SyncRoot)
            {
                if (!_Services.Contains (key))
                {
                    throw new NotSupportedException ("Service not supported: " + key);
                }

                return _Services [key];
            }
	    }
	}
}
