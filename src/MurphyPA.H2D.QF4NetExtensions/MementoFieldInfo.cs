using System;

namespace qf4net
{
	/// <summary>
	/// MementoFieldInfo.
	/// </summary>
	[Serializable]
	public class MementoFieldInfo : IFieldInfo
	{
		public MementoFieldInfo (string name, object value, Type type)
		{
			_Name = name;
			_Value = value;
			_Type = type;
		}

		string _Name;
		object _Value;
		Type _Type;

		public string Name { get { return _Name; } }
		public object Value { get { return _Value; } }
		public Type Type { get { return _Type; } }
	}
}
