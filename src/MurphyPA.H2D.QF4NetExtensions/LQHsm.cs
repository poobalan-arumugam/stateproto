using System;
using System.Runtime.Serialization;
using qf4net;

namespace qf4net
{
	/// <summary>
	/// Summary description for LQHsm.
	/// </summary>
    [Serializable]
    public abstract class LQHsm : QHsm, ILQHsm, ILQHsmAdmin
    {
        #region Boiler plate static stuff
        protected static new TransitionChainStore s_TransitionChainStore = 
            new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static LQHsm ()
        {
            s_TransitionChainStore.ShrinkToActualSize();
        }
        protected override TransitionChainStore TransChainStore
        {
            get { return s_TransitionChainStore; }
        }
        #endregion

        string _UniqueName = Guid.NewGuid ().ToString ();
        public string Id { get { return _UniqueName; } }

        string _GroupId;
        public string GroupId { get { return _GroupId; } }

        string _UnderlyingObjectId;
        public string UnderlyingObjectId
        {
            get
            {
                return _UnderlyingObjectId;
            }
            set
            {
                _UnderlyingObjectId = value;
            }
        }
        
        public LQHsm () 
        {
        }

        public LQHsm (bool createEventManager) 
        {
            if (createEventManager)
            {
                _EventManager = new QSingleHsmEventManager (new QSystemTimer ());
            }
        }

        public LQHsm (string id, string groupId)
        {
            _UniqueName = id;
            _GroupId = groupId;
        }

        public LQHsm (string id, IQEventManager eventManager)
        {
            _UniqueName = id;
            _EventManager = eventManager;
        }

        public LQHsm (string id, string groupId, IQEventManager eventManager)
        {
            _UniqueName = id;
            _GroupId = groupId;
            _EventManager = eventManager;
        }

        public LQHsm (IQEventManager eventManager) 
        {
            _EventManager = eventManager;
        }

        public LQHsm (string id, IQHsmLifeCycleManager lifeCycleManager)
        {
            _UniqueName = id;
            RegisterWithLifeCycleManager (lifeCycleManager);
        }

        public LQHsm (string id, string groupId, IQHsmLifeCycleManager lifeCycleManager)
        {
            _UniqueName = id;
            _GroupId = groupId;
            RegisterWithLifeCycleManager (lifeCycleManager);
        }

        public LQHsm (string id, IQHsmExecutionContext executionContext)
        {
            _UniqueName = id;
            RegisterWithExecutionContext (executionContext);
        }

        public LQHsm (string id, string groupId, IQHsmExecutionContext executionContext)
        {
            _UniqueName = id;
            _GroupId = groupId;
            RegisterWithExecutionContext (executionContext);
        }       

        protected override void InitializeStateMachine()
        {
            if (_EventManager == null)
            {
                throw new InvalidOperationException ("An EventManager must be assigned to this Hsm before calling Init.");
            }
        }

        protected virtual void LocateServicesUsingExecutionContext ()
        {
            
        }

        IQHsmLifeCycleManager _LifeCycleManager;
        protected IQHsmLifeCycleManager LifeCycleManager { get { return _LifeCycleManager; } }

        public void RegisterWithLifeCycleManager (IQHsmLifeCycleManager lifeCycleManager)
        {
            if (_LifeCycleManager != null)
            {
                throw new InvalidOperationException ("LifeCycleManager must only be assigned once against an Hsm. Multiple LifeCycleManagers against one Hsm is not currently supported.");   
            }
            _LifeCycleManager = lifeCycleManager;
            _LifeCycleManager.RegisterHsm (this);
        }

        IQHsmExecutionContext _ExecutionContext;
        protected IQHsmExecutionContext ExecutionContext { get { return _ExecutionContext; } }

        public void RegisterWithExecutionContext (IQHsmExecutionContext executionContext)
        {
            if (_ExecutionContext != null)
            {
                throw new InvalidOperationException ("ExecutionContext must only be assigned once against an Hsm. Multiple ExecutionContext against one Hsm is not currently supported.");                   
            }
            _ExecutionContext = executionContext;
            RegisterWithLifeCycleManager (executionContext.LifeCycleManager);            

            // only makes sense to call this if using an ExecutionContext - since it provides a GetService () method.
            LocateServicesUsingExecutionContext ();
        }

        #region Instrumentation
        protected void LogStateEvent (StateLogType logType, QState state, QState initState)
        {
            System.Diagnostics.Debug.Assert (logType == StateLogType.Init);
            LogStateEventArgs logEvent = new LogStateEventArgs ();
            logEvent.LogType = logType;
            logEvent.State = state;
            logEvent.NextState = initState;
            DoStateChange (logEvent);
        }

        protected void LogStateEvent (StateLogType logType, QState state, string action)
        {
            LogStateEventArgs logEvent = new LogStateEventArgs ();
            logEvent.LogType = logType;
            logEvent.State = state;
            logEvent.LogText = action;
            DoStateChange (logEvent);
        }

        protected void LogStateEvent (StateLogType logType, QState state)
        {
            LogStateEventArgs logEvent = new LogStateEventArgs ();
            logEvent.LogType = logType;
            logEvent.State = state;
            DoStateChange (logEvent);
        }

        protected void LogStateEvent (StateLogType logType, QState state, QState toState, string eventName, string eventDescription)
        {
            LogStateEventArgs logEvent = new LogStateEventArgs ();
            logEvent.LogType = logType;
            logEvent.State = state;
            logEvent.NextState = toState;
            logEvent.EventName = eventName;
            logEvent.EventDescription = eventDescription;
            DoStateChange (logEvent);
        }

        protected void LogText (string fmt, params object[] args)
        {
            string msg;
            if (args == null || args.Length == 0)
            {
                msg = fmt;
            } 
            else 
            {
                msg = string.Format (fmt, args);
            }
            LogStateEventArgs logEvent = new LogStateEventArgs ();
            logEvent.LogType = StateLogType.Log;
            logEvent.LogText = msg;
            DoStateChange (logEvent);
        }

        protected virtual bool OnStateChange (LogStateEventArgs logEvent)
        {
            return true;
        }

        protected void RaiseStateChange (EventHandler handler, ILQHsm hsm, LogStateEventArgs logEvent)
        {
            if (handler != null)
            {
                handler (hsm, logEvent);
            }
        }

        protected void DoStateChange (LogStateEventArgs logEvent)
        {
            if (OnStateChange (logEvent))
            {
                RaiseStateChange (StateChange, this, logEvent);
            }
        }

        public event EventHandler StateChange;
        #endregion

        #region FinalStateReached event
        protected virtual void RaiseFinalStateReached (EventHandler handler, ILQHsm hsm, QState state)
        {
            if (handler != null)
            {
                LogStateEventArgs logEvent = new LogStateEventArgs ();
                logEvent.LogType = StateLogType.Final;
                logEvent.State = state;
                handler (hsm, logEvent);                
            }
        }

        protected virtual bool OnFinalStateReached (ILQHsm hsm, QState state)
        {
            return true;
        }

        protected void DoFinalStateReached (ILQHsm hsm, QState state)
        {
            if (OnFinalStateReached (hsm, state))
            {
                LogStateEvent (StateLogType.Final, state);
                RaiseFinalStateReached (FinalStateReached, hsm, state);
            }
        }
        public event EventHandler FinalStateReached;
        #endregion

		#region Event Management
		IQEventManager _EventManager;
		public IQEventManager EventManager
		{
		    get { return _EventManager; }
            set
            {
                if (_EventManager != null)
                {
                    throw new InvalidOperationException ("EventManager already assigned to this Machine. Once assigned no other set operation is permitted.");
                }
                _EventManager = value; 
            }
		}

		public new void Dispatch (IQEvent ev)
		{
			_EventManager.Dispatch (this, ev);
		}

		public void AsyncDispatch (IQEvent ev)
		{
			_EventManager.AsyncDispatch (this, ev);
		}

		public void AsyncDispatchFront (IQEvent ev)
		{
			_EventManager.AsyncDispatchFront (this, ev);
		}
		#endregion

		#region TimeOut

		protected string ToTimeOutName (string name)
		{
			string newName = _UniqueName + name;
			return newName;
		}

		public void SetTimeOut (string name, TimeSpan duration, IQEvent ev)
		{
			_EventManager.SetTimeOut (ToTimeOutName (name), duration, this, ev);
		}

		public void SetTimeOut (string name, TimeSpan duration, IQEvent ev, TimeOutType timeOutType)
		{
			_EventManager.SetTimeOut (ToTimeOutName (name), duration, this, ev, timeOutType);
		}

		public void SetTimeOut (string name, DateTime at, IQEvent ev)
		{
			_EventManager.SetTimeOut (ToTimeOutName (name), at, this, ev);
		}

		public void SetTimeOut (string name, DateTime at, IQEvent ev, TimeOutType timeOutType)
		{
			_EventManager.SetTimeOut (ToTimeOutName (name), at, this, ev, timeOutType);
		}

		public void ClearTimeOut (string name)
		{
			_EventManager.ClearTimeOut (ToTimeOutName (name));
		}
		#endregion

        public virtual bool IsFinalState (QState state)
        {
            return false;
        }

		ModelInformation _ModelInformation;
		public ModelInformation ModelInformation 
		{ 
			get 
			{
				if (_ModelInformation == null)
				{
					object[] modelInformationList = this.GetType ().GetCustomAttributes (typeof (ModelInformationAttribute), false);
					if (modelInformationList.Length != 1)
					{
						string msg = string.Format ("One ModelInformation Attribute must be defined on this Hsm [{0}] - currently there are {1} defined", this, modelInformationList.Length);
						throw new ArgumentException (msg);
					}
					ModelInformationAttribute modelInformation = (ModelInformationAttribute) modelInformationList [0];
					_ModelInformation = modelInformation.ModelInformation;
				}
				return _ModelInformation;
			}
		}

		public TransitionEventAttribute[] TransitionEvents 
		{
			get
			{
				object[] list = GetType ().GetCustomAttributes (typeof (TransitionEventAttribute), true);
				if (list == null)
				{
					list = new object [] {};
				}
				TransitionEventAttribute[] teList = new TransitionEventAttribute[list.Length];
				if (list.Length > 0)
				{
					list.CopyTo (teList, 0);
				}
				return teList;
			}
		}

		public override string ToString()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder ();
            sb.Append (base.ToString ());
            sb.Append ("[");
            sb.Append (GetHashCode ());
            if (_UnderlyingObjectId != null)
            {
                sb.Append ("->");
                sb.Append (_UnderlyingObjectId);
            }
            if (_GroupId != null)
            {
                sb.Append ("=>");
                if (_GroupId == _UnderlyingObjectId)
                {
                    sb.Append (".");
                } 
                else 
                {
                    sb.Append (_GroupId);
                }
            }
            sb.Append ("]");
            string sv = sb.ToString ();          
			return sv;
		}

		protected virtual IQMultiPort CreateMultiPort (string name)
		{
			IQMultiPort port = new QMultiPort (name, this); 
			return port;
		}

		protected virtual IQPort CreatePort (string name)
		{
			IQPort port = new QPort (name, this); 
			return port;
		}

		#region State Serialisation
		public void Serialise (ISerialisationContext context)
		{
			context.Formatter.Serialize (context.Stream, _UniqueName);
			context.Formatter.Serialize (context.Stream, this.CurrentStateMethod);
			SaveHistoryStates (context);
			SaveFields (context);
		}

        public void Deserialise (ISerialisationContext context)
        {
            this._UniqueName = (string) context.Formatter.Deserialize (context.Stream); 
            this.CurrentStateMethod  = (System.Reflection.MethodInfo) context.Formatter.Deserialize (context.Stream);
            LoadHistoryStates (context);
            LoadFields (context);

            LogRestored ();
        }

		protected virtual void SaveHistoryStates (ISerialisationContext context)
		{
		}

		protected virtual void LoadHistoryStates (ISerialisationContext context)
		{
		}

		protected virtual void SaveFields (ISerialisationContext context)
		{
		}

		protected virtual void LoadFields (ISerialisationContext context)
		{
		}
		#endregion

		#region Memento
		public void SaveToMemento (ILQHsmMemento memento)
		{
            ModelInformation modelInformation = this.ModelInformation;
            memento.ModelVersion = modelInformation.ModelVersion;
            memento.ModelGuid = modelInformation.Guid;

            memento.Id = this.Id;
            memento.GroupId = this.GroupId;
            memento.CurrentStateMethod = this.CurrentStateMethod;

            SaveHistoryStates (memento);
			SaveFields (memento);
		}

		public void RestoreFromMemento (ILQHsmMemento memento)
		{
            ModelInformation modelInformation = this.ModelInformation;
            if (modelInformation.Guid != memento.ModelGuid)
            {
                string msg = string.Format ("Hsm Guid [{0}] is not the same as contained by the Memento [{1}]", modelInformation.Guid, memento.ModelGuid);
                throw new InvalidOperationException (msg);                
            }

			_UniqueName = memento.Id;
            _GroupId = memento.GroupId;
			this.CurrentStateMethod = memento.CurrentStateMethod;

			RestoreHistoryStates (memento);
			RestoreFields (memento);

            LogRestored ();
		}

        protected virtual void LogRestored ()
        {
            Delegate stateDelegate = Delegate.CreateDelegate (typeof (QState), this, CurrentStateMethod.Name);
            QState currentState = (QState) stateDelegate;
            LogStateEvent (StateLogType.Restored, currentState);
        }

		protected virtual void SaveHistoryStates (ILQHsmMemento memento)
		{
		}

		protected virtual void SaveFields (ILQHsmMemento memento)
		{
		}

		protected virtual void RestoreHistoryStates (ILQHsmMemento memento)
		{
		}

		protected virtual void RestoreFields (ILQHsmMemento memento)
		{
		}

		#endregion
	}

	[Serializable]
	public enum StateLogType { Init, Entry, Exit, EventTransition, EventInternalTransition, Log, Final, Restored };

	[Serializable]
	public class LogStateEventArgs : EventArgs 
	{
		StateLogType _LogType;
		public StateLogType LogType { get { return _LogType; } set { _LogType = value; } }

		QState _State;
		QState _NextState;

		public QState State { get { return _State; } set { _State = value; } }
		public QState NextState { get { return _NextState; } set { _NextState = value; } }

		string _EventName;
		public string EventName { get { return _EventName; } set { _EventName = value; } }

		string _EventDescription;
		public string EventDescription { get { return _EventDescription; } set { _EventDescription = value; } }

		string _LogText;
		public string LogText { get { return _LogText; } set { _LogText = value; } }
	}
}
