using System;
using System.Collections;

namespace qf4net
{
	/// <summary>
	/// Summary description for LQHsmWithSubMachines.
	/// </summary>
	[Serializable]
	public abstract class LQHsmWithSubMachines : LQHsm, IQSupportsSubMachines
	{
		#region Boiler plate static stuff
		protected static new TransitionChainStore s_TransitionChainStore = 
			new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		static LQHsmWithSubMachines ()
		{
			s_TransitionChainStore.ShrinkToActualSize();
		}
		protected override TransitionChainStore TransChainStore
		{
			get { return s_TransitionChainStore; }
		}
		#endregion

		public LQHsmWithSubMachines ()
			: base () {}

		public LQHsmWithSubMachines (IQEventManager eventManager)
			: base (eventManager) {}

		#region IQSupportsSubMachines Members

		ILQHsmDictionary _SubMachines = new ILQHsmDictionary ();

		protected ILQHsmDictionary InternalSubMachines 
		{
			get { return _SubMachines; }
		}
	
		public System.Collections.IEnumerable SubMachines
		{
			get
			{
				return _SubMachines;
			}
		}

		#endregion
	}
}
