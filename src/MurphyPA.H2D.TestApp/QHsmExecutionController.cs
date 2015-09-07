using System;
using System.Collections;
using MurphyPA.H2D.Interfaces;
using qf4net;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for QHsmExecutionController.
	/// </summary>
	public class QHsmExecutionController : ConvertToCodeBase, IDisposable, IQStateChangeListener
	{
		ILQHsm _Hsm;
		IQStateChangeListener _Listener;

		public QHsmExecutionController(DiagramModel model)
			: base (model.GetGlyphsList ())
		{
		}

		protected void Prepare ()
		{
			PrepareGlyphs ();
			foreach (IGlyph glyph in _Glyphs)
			{
				glyph.Selected = false;
			}
		}

		protected void InitInstrumentation (ILQHsm hsm)
		{
			// Use QStateChangeListener to minimise exposure to this execution controller - I do not want the controller
			// ref to be passed along.
			_Listener = new QStateChangeListenerBase (this);

			if (_Hsm != null)
			{
				_Hsm.StateChange -= new EventHandler (_Listener.HandleStateChange);
			}
			_Hsm = hsm;
			_Hsm.StateChange += new EventHandler(_Listener.HandleStateChange);
		}

		public void Execute (ILQHsm hsm) 
		{
			Prepare ();

			InitInstrumentation (hsm);

			_Hsm.Init ();

			if (_Hsm.EventManager.Runner == null)
			{
				new QGUITimerEventManagerRunner (_Hsm.EventManager, 200);
				_Hsm.EventManager.Runner.Start ();
			}
		}

		public ILQHsm Hsm { get { return _Hsm; } }

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
					_CurrentState.Selected = true;
				}
			}
		}

		ITransitionGlyph _CurrentTransition;
		protected ITransitionGlyph CurrentTransition 
		{
			get { return _CurrentTransition; }
			set 
			{
				if (_CurrentTransition != null)
				{
					_CurrentTransition.Selected = false;
				}
				_CurrentTransition = value; 
				if (_CurrentTransition != null)
				{
					_CurrentTransition.Selected = true;
				}
			}
		}

		protected void SetCurrentStateName (string stateName)
		{
			CurrentState = null;
			foreach (IStateGlyph state in _States)
			{
				string loopStateName = StateNameFrom (state);
				if (stateName == loopStateName)
				{
					CurrentState = state;
				}
			}
		}

		protected void SetCurrentTransitionName (string stateName, string eventDesc)
		{
			CurrentTransition = null;
			foreach (IStateGlyph state in _States)
			{
				string loopStateName = StateNameFrom (state);
				if (stateName == loopStateName)
				{
					ArrayList transitions = GetTransitionList (state);
					foreach (TransitionInfo info in transitions)
					{
						if (info.Transition.DisplayText () == eventDesc)
						{
							CurrentTransition = info.Transition;
						}
					}
				}
			}
		}

		protected string QStateNameFrom (QState state)
		{
			string stateName = state.Method.Name.Remove(0, 2);
			return stateName;
		}

		protected void hsm_StateChange(object sender, EventArgs e)
		{
			LogStateEventArgs args = e as LogStateEventArgs;
			ILQHsm hsm = sender as ILQHsm;

			if (args.LogType == StateLogType.Log)
			{
				return;
			}

			string stateName = QStateNameFrom (args.State);
			switch (args.LogType)
			{
				case StateLogType.Init: 
				{
					SetCurrentStateName (stateName);
				} break;
				case StateLogType.Entry:
				{
					SetCurrentStateName (stateName);
				} break;
				case StateLogType.Exit:			
				{
					CurrentState = null;
				} break;
				case StateLogType.EventTransition: 
				{
					CurrentTransition = null;
					DoRefresh ();
					SetCurrentStateName (stateName);
					SetCurrentTransitionName (stateName, args.EventDescription);
					DoRefresh ();
				} break;
				case StateLogType.Log: 
				{
				} break;
				default: throw new NotSupportedException ("StateLogType." + args.LogType.ToString ());
			}

			DoRefresh ();
		}

		public event EventHandler Refresh;

		void DoRefresh ()
		{
			if (Refresh != null)
			{
				Refresh (this, new EventArgs ());
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_Hsm != null)
			{
				_Hsm.EventManager.Runner.Stop ();
				_Hsm.EventManager.Runner = null;
				_Hsm = null;
			}
		}

		#endregion

		#region IQStateChangeListener Members

		public void HandleStateChange(object sender, EventArgs e)
		{
			hsm_StateChange (sender, e);
		}

		#endregion
	}
}
