using System;
using System.Collections;

namespace qf4net
{
	/// <summary>
	/// QHsmLifeCycleManagerBase.
	/// </summary>
	public class QHsmLifeCycleManagerBase : QHsmLifeCycleManagerEventsBase, IQHsmLifeCycleManager
	{
		public QHsmLifeCycleManagerBase()
		{
        }

        Hashtable _HsmCollection = new Hashtable ();

	    public void RegisterHsm(ILQHsm hsm)
	    {
            lock (_HsmCollection.SyncRoot)
            {
                _HsmCollection.Add (hsm.Id, hsm);   
            }
	        DoLifeCycleChange (this, hsm, QHsmLifeCycleChangeType.Added);
	    }

	    public void UnregisterHsm(ILQHsm hsm)
	    {
            bool contains = false;
            lock (_HsmCollection.SyncRoot)
            {
                contains = _HsmCollection.Contains (hsm.Id);
                if (contains)
                {
                    _HsmCollection.Remove (hsm.Id);   
                }
            }	 
            if (contains)
            {
                DoLifeCycleChange (this, hsm, QHsmLifeCycleChangeType.Removed);
            }
        }
	}
}
