using System;
using System.Windows.Forms;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for UIGlyphKeyboardInputInteractor.
	/// </summary>
	public class UIGlyphKeyboardInputInteractor : UIGlyphInteractionHandlerBase
	{
		protected IGlyph _LastSelectedGlyph;

		public UIGlyphKeyboardInputInteractor (IUIInterationContext context)
			: base (context) {}

		public override void KeyUp(object sender, KeyEventArgs e)
		{
			if (_LastSelectedGlyph == null)
			{
				return;
			}

			if (_Model.IsStateGlyph (_LastSelectedGlyph))
			{
				if (IsControlKey (e, Keys.T)) // Toggle Start State
				{
					IStateGlyph stateGlyph = _LastSelectedGlyph as IStateGlyph;
					stateGlyph.IsStartState = !stateGlyph.IsStartState;
				}
				if (IsControlKey (e, Keys.N)) // name
				{
					_LastSelectedGlyph.Name = InputDialog.Execute (_Context.ParentStateDiagramView, "State Property Input", "State Name", _LastSelectedGlyph.Name);				
				}
				else if (IsControlKey (e, Keys.E)) // entry action
				{
					IStateGlyph stateGlyph = _LastSelectedGlyph as IStateGlyph;
					stateGlyph.EntryAction = InputDialog.Execute (_Context.ParentStateDiagramView, "State Property Input", "State Entry Action", stateGlyph.EntryAction);
				}
				else if (IsControlKey (e, Keys.X)) // exit action
				{
					IStateGlyph stateGlyph = _LastSelectedGlyph as IStateGlyph;
					stateGlyph.ExitAction = InputDialog.Execute (_Context.ParentStateDiagramView, "State Property Input", "State Exit Action", stateGlyph.ExitAction);
				}
                else if (IsControlKey (e, Keys.F))
                {
                    IStateGlyph stateGlyph = _LastSelectedGlyph as IStateGlyph;
                    stateGlyph.IsFinalState = !stateGlyph.IsFinalState;
                }
            }
			else if (_Model.IsTransitionGlyph (_LastSelectedGlyph))
			{
				if (IsControlKey (e, Keys.N))
				{
					_LastSelectedGlyph.Name = InputDialog.Execute (_Context.ParentStateDiagramView, "Transition Property Input", "Transition Name", _LastSelectedGlyph.Name);
				}
				else if (IsControlKey (e, Keys.V))
				{
					ITransitionGlyph transitionGlyph = _LastSelectedGlyph as ITransitionGlyph;
					transitionGlyph.EventSignal = InputDialog.Execute (_Context.ParentStateDiagramView, "Transition Property Input", "Transition Event Signal", transitionGlyph.EventSignal);
				}
				else if (IsControlKey (e, Keys.K))
				{
					ITransitionGlyph transitionGlyph = _LastSelectedGlyph as ITransitionGlyph;
					transitionGlyph.EventSource = InputDialog.Execute (_Context.ParentStateDiagramView, "Transition Property Input", "Transition Event Source", transitionGlyph.EventSource);
				}
				else if (IsControlKey (e, Keys.G))
				{
					ITransitionGlyph transitionGlyph = _LastSelectedGlyph as ITransitionGlyph;
					transitionGlyph.GuardCondition = InputDialog.Execute (_Context.ParentStateDiagramView, "Transition Property Input", "Transition Event Guard Condition", transitionGlyph.GuardCondition);
				}
				else if (IsControlKey (e, Keys.A))
				{
					ITransitionGlyph transitionGlyph = _LastSelectedGlyph as ITransitionGlyph;
					transitionGlyph.Action = InputDialog.Execute (_Context.ParentStateDiagramView, "Transition Property Input", "Transition Event Action", transitionGlyph.Action);
				}
				else if (IsControlKey (e, Keys.I))
				{
					ITransitionGlyph transitionGlyph = _LastSelectedGlyph as ITransitionGlyph;
					transitionGlyph.IsInnerTransition = !transitionGlyph.IsInnerTransition;
				}
            }
			else if (_Model.IsStateTransitionPortGlyph (_LastSelectedGlyph))
			{
				if (IsControlKey (e, Keys.N))
				{
					_LastSelectedGlyph.Name = InputDialog.Execute (_Context.ParentStateDiagramView, "State Transition Port Property Input", "Port Name", _LastSelectedGlyph.Name);
				}
			}
            else if (_Model.IsComponentGlyph (_LastSelectedGlyph))
            {
                if (IsControlKey (e, Keys.N))
                {
                    _LastSelectedGlyph.Name = InputDialog.Execute (_Context.ParentStateDiagramView, "Component Property Input", "Component Name", _LastSelectedGlyph.Name);
                }                
            }
            else 
            {
                if (IsControlKey (e, Keys.N))
                {
                    _LastSelectedGlyph.Name = InputDialog.Execute (_Context.ParentStateDiagramView, "Property Input", "Name", _LastSelectedGlyph.Name);
                }                
            }

			_Context.RefreshView ();
		}
	}
}
