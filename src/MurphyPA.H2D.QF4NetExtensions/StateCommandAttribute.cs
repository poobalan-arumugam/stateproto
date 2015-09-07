using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for StateCommandAttribute.
	/// </summary>
	[AttributeUsage (AttributeTargets.Method, AllowMultiple=true)]
	public class StateCommandAttribute : Attribute
	{
		public StateCommandAttribute(string commandName)
		{
			_CommandName = commandName;
		}

		#region CommandName
		private string _CommandName;
		public string CommandName { get { return _CommandName; } set { _CommandName = value; } }
		#endregion
	}
}
