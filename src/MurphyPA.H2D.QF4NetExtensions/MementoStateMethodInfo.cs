using System;
using System.Reflection;

namespace qf4net
{
	/// <summary>
	/// MementoStateMethodInfo.
	/// </summary>
	[Serializable]
	public class MementoStateMethodInfo : IStateMethodInfo
	{
		public MementoStateMethodInfo(string name, MethodInfo method)
		{
			_Name = name;
			_Method = method;
		}

		string _Name;
		public string Name { get { return _Name; } }

		MethodInfo _Method;
		public MethodInfo Method { get { return _Method; } }
	}
}
