using System;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for HsmUtil.
	/// </summary>
	public class HsmUtil
	{
		public static qf4net.ILQHsm CreateHsm (Type type)
		{
			qf4net.IQTimer timer = new qf4net.QSystemTimer ();
			qf4net.QSingleHsmEventManager eventManager = new qf4net.QSingleHsmEventManager (timer);
			object ohsm = Activator.CreateInstance (type, new object[] {eventManager});
			//object ohsm = _AppDomain.CreateInstanceAndUnwrap (assemblyName, typeName);
			qf4net.ILQHsm hsm = ohsm as qf4net.ILQHsm;
			return hsm;
		}
	}
}
