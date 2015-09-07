using System;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ExecutionController.
	/// </summary>
	public class ExecutionController : ConvertToCodeBase
	{
		ArrayList _TransitionList;
		Queue _EventQueue;
		Hashtable _DeepHistory;

		public Queue EventQueue { get { return _EventQueue; } }

		public ExecutionController (ArrayList glyphs)
			: base (glyphs)
		{
		}

		public void InjectEvent (string eventName)
		{
			_EventQueue.Enqueue (eventName);
		}

		protected void Prepare ()
		{
			_EventQueue = new Queue ();
			_DeepHistory = new Hashtable ();
			PrepareGlyphs ();
			foreach (IGlyph glyph in _Glyphs)
			{
				glyph.Selected = false;
			}
		}

		public void Start ()
		{
			Prepare ();

			IStateGlyph state = OuterMostState ();
			if (state == null)
			{
				throw new Exception ("No parent start state found!");
			}
			CurrentState = state;
			DoRefresh ();
		}

		public void Step ()
		{
			if (CurrentState == null)
			{
				Start ();
				return;
			}

			if (!FindInnerStartState ()) 
			{
				if (!StepNextEvent ())
				{
				}
			}

			DoRefresh ();
		}

		public void Stop ()
		{
			CurrentState = null;
			Prepare ();
		}

		bool FindInnerStartState ()
		{
			if (CurrentState == null) return false;

			foreach (IGlyph glyph in CurrentState.Children)
			{
				IStateGlyph state = glyph as IStateGlyph;
				if (state != null && state.IsStartState)
				{
					CurrentState = state;
					return true;
				}
			}

			return false;
		}

		protected bool StepNextEvent ()
		{
			if (_EventQueue.Count == 0) 
			{
				DoNoEvents ();
				return false;
			}

			string eventName = _EventQueue.Dequeue () as string;

			foreach (TransitionInfo info in _TransitionList)
			{
				if (info.Transition.Event == eventName)
				{
					DoTransitionEvent (eventName);
					switch (info.Transition.TransitionType)
					{
						case TransitionType.History: 
						{
							throw new NotSupportedException ("History not supported");
						} break;
						case TransitionType.DeepHistory: 
						{
							IStateGlyph toState = _DeepHistory [info.ToStateGlyph] as IStateGlyph;
							if (toState == null)
							{
								toState = info.ToStateGlyph;
							} 
							CurrentState = toState;
						} break;
						default: 
						{
							CurrentState = info.ToStateGlyph;
						} break;
					}
					return true;
				}
			}

			DoDropEvent (eventName);

			return false;
		}


		IStateGlyph _CurrentState;
		protected IStateGlyph CurrentState 
		{
			get { return _CurrentState; }
			set 
			{
				if (_CurrentState != null)
				{
					_CurrentState.Selected = false;
				}
				_CurrentState = value; 
				if (_CurrentState != null)
				{
					CreateNewTransitionList (_CurrentState);
					_CurrentState.Selected = true;
					UpdateAllParentsDeepHistory ();
				}
			}
		}

		protected void UpdateAllParentsDeepHistory ()
		{
			IStateGlyph state = _CurrentState;
			while (state != null)
			{
				_DeepHistory [state] = _CurrentState;
				state = state.Parent as IStateGlyph;
			}
		}

		protected void CreateNewTransitionList (IStateGlyph state)
		{
			ArrayList list = new ArrayList ();
			while (state != null)
			{
				GetTransitionList (list, state);
				IGlyph parent =	state.Parent;
				state = parent as IStateGlyph;
				if (parent != null)
				{
					System.Diagnostics.Debug.Assert (state != null); 
				}
			}
			_TransitionList = list;
			DoNewTransitionList (_CurrentState, list);
		}

		public event EventHandler Refresh;

		void DoRefresh ()
		{
			if (Refresh != null)
			{
				Refresh (this, new EventArgs ());
			}
		}

		public class TransitionListEventArgs : EventArgs 
		{
			IStateGlyph _State;
			public IStateGlyph State { get { return _State; } }

			ArrayList _TransitionList;
			public ArrayList TransitionList { get { return _TransitionList; } }

			public TransitionListEventArgs (IStateGlyph state, ArrayList transitionList)
			{
				_State = state;
				_TransitionList = transitionList;
			}
		}

		public event EventHandler NewTransitionList;

		void DoNewTransitionList (IStateGlyph state, ArrayList transitionList)
		{
			if (NewTransitionList != null)
			{
				NewTransitionList (this, new TransitionListEventArgs (state, transitionList));
			}
		}

		public event EventHandler NoEvents;

		protected void DoNoEvents ()
		{
			if (NoEvents != null)
			{
				NoEvents (this, new EventArgs ());
			}
		}

		public class EventNameEventArgs : EventArgs 
		{
			public EventNameEventArgs (string eventName)
			{
				_EventName = eventName;
			}

			string _EventName;
			public string EventName { get { return _EventName; } }
		}

		public event EventHandler DropEvent;

		protected void DoDropEvent (string eventName)
		{
			if (DropEvent != null)
			{
				DropEvent (this, new EventNameEventArgs (eventName));
			}
		}

		public event EventHandler TransitionEvent;

		protected void DoTransitionEvent (string eventName)
		{
			if (TransitionEvent != null)
			{
				TransitionEvent (this, new EventNameEventArgs (eventName));
			}
		}
	}
}
