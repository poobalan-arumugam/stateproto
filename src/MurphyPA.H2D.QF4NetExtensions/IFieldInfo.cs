using System;

namespace qf4net
{
	/// <summary>
	/// IFieldInfo.
	/// </summary>
	public interface IFieldInfo
	{
		string Name { get; }
		object Value { get; }
		Type Type { get; }
	}
}
