using System;
using System.Collections;
using MurphyPA.H2D.Interfaces;
using System.IO;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ActionAnalyser.
	/// </summary>
	public class ActionAnalyser : ConvertToCodeBase
	{
		public ActionAnalyser(ArrayList states)
			: base (states)
		{
		}

		public void Analyse ()
		{
			PrepareGlyphs ();

			foreach (IStateGlyph state in _States)
			{
				AnalyseAction (state, state.EntryAction);
				AnalyseAction (state, state.ExitAction);
				AnalyseAction (state, state.DoAction);

				
				ArrayList transitionList = GetTransitionList (state);
				ArrayList groupedTransitions = GroupTransitionsByEvent (transitionList);
				foreach (ArrayList groupedTransitionList in groupedTransitions)
				{
					AnalyseTransition (state, groupedTransitionList);
				}
			}
		}

		protected void AnalyseTransition (IStateGlyph state, ArrayList groupedTransitionList)
		{
			foreach (TransitionInfo transInfo in groupedTransitionList)
			{
				ITransitionGlyph trans = transInfo.Transition;

				if (HasGuardCondition (trans))
				{
					AnalyseGuardAction (state, trans, trans.GuardCondition);
				}

				if (IsNotEmptyString (trans.Action))
				{
					AnalyseAction (state, trans, trans.Action);
				}
			}
		}

		protected void AnalyseAction (IStateGlyph state, string actions)
		{            
            if (IsNotEmptyString (actions))
            {
                string[] names = ParseNames (actions);
            }
		}

		protected void AnalyseAction (IStateGlyph state, ITransitionGlyph trans, string actions)
		{
			string[] names = ParseNames (actions);
		}

		protected void AnalyseGuardAction (IStateGlyph state, ITransitionGlyph trans, string actions)
		{
			string[] names = ParseNames (actions);
		}

		protected string[] ParseNames (string actions)
		{
			char[] buffer = new char [1];
			StringReader sr = new StringReader (actions);
			while (sr.Peek () != -1)
			{
				sr.Read (buffer, 0, 1);
			}

			return new string[] {};
		}

	}
}
