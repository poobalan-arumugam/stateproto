
using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for TransitionEventAttribute.
	/// </summary>
	[AttributeUsage (AttributeTargets.Class, @AllowMultiple=true)]
	public class TransitionEventAttribute : Attribute
	{
		string _EventName;
		string _EventSource;
		Type _DataType;

		public TransitionEventAttribute (string eventName)
		{
			_EventName = eventName;
		}

		public TransitionEventAttribute (string eventName, Type dataType)
		{
			_EventName = eventName;
			_DataType = dataType;
		}

		public TransitionEventAttribute (string eventName, string eventSource)
		{
			_EventName = eventName;
			_EventSource = eventSource;
		}

		public TransitionEventAttribute (string eventName, string eventSource, Type dataType)
		{
			_EventName = eventName;
			_EventSource = eventSource;
			_DataType = dataType;
		}

		public bool HasDataType { get { return _DataType != null; } }

		public string EventName { get { return _EventName; } }
		public string EventSource { get { return _EventSource; } }
		public Type DataType { get { return _DataType; } }

		public override string ToString()
		{
			if (EventSource != null)
			{
				if (DataType != null)
				{
					return string.Format ("{0}.{1}/{2}", EventSource, EventName, DataType);
				} 
				else 
				{
					return string.Format ("{0}.{1}", EventSource, EventName);
				}
			} 
			else 
			{
				if (DataType != null)
				{
					return string.Format ("{0}/{1}", EventName, DataType);
				} 
				else 
				{
					return EventName;
				}
			}
		}

	}
}
