using System;
using System.Reflection;
using System.Collections;

namespace qf4net
{
	/// <summary>
	/// LQHsmMemento.
	/// </summary>
	[Serializable]
	public class LQHsmMemento : ILQHsmMemento
	{
		#region ILQHsmMemento Members

		string _Id;
		public string Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

        string _GroupId;
        public string GroupId
        {
            get
            {
                return _GroupId;
            }
            set
            {
                _GroupId = value;
            }
        }

        string _ModelVersion;
        public string ModelVersion
        {
            get
            {
                return _ModelVersion;
            }
            set
            {
                _ModelVersion = value;
            }           
        }

        string _ModelGuid;
        public string ModelGuid
        {
            get 
            {
                return _ModelGuid; 
            }
            set
            {
                _ModelGuid = value;
            }
        }

		MethodInfo _CurrentStateMethod;
		public MethodInfo CurrentStateMethod
		{
			get
			{
				return _CurrentStateMethod;
			}
			set
			{
				_CurrentStateMethod = value;
			}
		}

		Hashtable _HistoryStates = null;

		public void ClearHistoryStates()
		{
			_HistoryStates = null;
		}

        /// <summary>
        /// Added Allow_GetHistoryState_CallWithoutSupplyingHistory() call for Hsm's 
        /// that has history states - but for which the developer does not want 
        /// to supply any when hand constructing a memento. This is a very special case!
        /// 
        /// This could be done via an extra flag field - but that would be memory
        /// inefficient for a special case scenario like this.
        /// 
        /// All that is needed is that GetHistoryStateFor() must work and return 
        /// a null state. 
        /// 
        /// Note: By default - if the memento was properly filled in by the LQHsm
        /// (via a call to LQHsm::SaveHistoryStates() then this call would not be needed.
        /// </summary>
        public void Allow_GetHistoryState_CallWithoutSupplyingHistory()
        {
            SetupHistoryStatesContainer ();
        }

        private void SetupHistoryStatesContainer()
        {
            if (_HistoryStates == null)
            {
                _HistoryStates = new Hashtable ();
            }
        }

		public void AddHistoryState(string name, System.Reflection.MethodInfo state)
		{
            SetupHistoryStatesContainer();
			_HistoryStates.Add (name, new MementoStateMethodInfo (name, state));
		}

		Hashtable _Fields = null;

		public void ClearFields()
		{
			_Fields = null;
		}

		public void AddField(string name, object value, Type type)
		{
			if (_Fields == null)
			{
				_Fields = new Hashtable ();
			}

			_Fields.Add (name, new MementoFieldInfo (name, value, type));
		}

		public IStateMethodInfo GetHistoryStateFor (string name)
		{
			if (_HistoryStates == null)
			{
				throw new NullReferenceException ("No History States are being held by this memento");
			}

			IStateMethodInfo methodInfo = _HistoryStates [name] as IStateMethodInfo;
			return methodInfo;
		}

		public IFieldInfo GetFieldFor (string name)
		{
			if (_Fields == null)
			{
				throw new NullReferenceException ("No Fields are being held by this memento");
			}

			IFieldInfo fieldInfo = _Fields [name] as IFieldInfo;
			return fieldInfo;
		}

		public IStateMethodInfo[] GetHistoryStates ()
		{
			if (_HistoryStates == null)
			{
				return null;
			}

			ArrayList list = new ArrayList (_HistoryStates.Values);
			IStateMethodInfo[] infos = (IStateMethodInfo[]) list.ToArray (typeof (IStateMethodInfo));
			return infos;
		}

		public IFieldInfo[] GetFields ()
		{
			if (_Fields == null)
			{
				return null;
			}

			ArrayList list = new ArrayList (_Fields.Values);
			IFieldInfo[] infos = (IFieldInfo[]) list.ToArray (typeof (IFieldInfo));
			return infos;
		}

		#endregion
	}
}
