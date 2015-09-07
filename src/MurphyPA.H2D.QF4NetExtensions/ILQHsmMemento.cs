using System;
using System.Reflection;
using System.Collections;

namespace qf4net
{
	/// <summary>
	/// ILQHsmMemento.
	/// </summary>
	public interface ILQHsmMemento
	{
		string Id { get; set; }
        string GroupId { get; set; }
        string ModelVersion { get; set; }
        string ModelGuid { get; set; }
		MethodInfo CurrentStateMethod { get; set; }
		void ClearHistoryStates ();
		void AddHistoryState (string name, MethodInfo state);
		void ClearFields ();
		void AddField (string name, object value, Type type); // add more type specific fields in future.

		IStateMethodInfo GetHistoryStateFor (string name);
		IFieldInfo GetFieldFor (string name);

		IStateMethodInfo[] GetHistoryStates ();
		IFieldInfo[] GetFields ();
	}
}
