using System;
using System.Reflection;

namespace qf4net
{
	/// <summary>
	/// IStateMethodInfo.
	/// </summary>
	public interface IStateMethodInfo
	{
		string Name { get; }
		MethodInfo Method { get; }
	}
}
