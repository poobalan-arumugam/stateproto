using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for StateMethod.
	/// </summary>
	[AttributeUsage (AttributeTargets.Method)]
	public class StateMethodAttribute : Attribute
	{
		public StateMethodAttribute (string name)
		{
			_StateName = name;
		}

		#region StateName
		private string _StateName;
		public string StateName { get { return _StateName; } set { _StateName = value; } }
		#endregion
	}
}
