using System;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ConvertToCodeBase.
	/// </summary>
	public class ConvertToCodeBase : LoggingUserBase
	{
		protected ArrayList _SourceGlyphs;
		protected ArrayList _Glyphs;
		protected ArrayList _States;
		protected ArrayList _Transitions;
		protected ArrayList _Ports;
		protected Hashtable _StatesWithHistoryTransitions;

		public ConvertToCodeBase (ArrayList glyphs)
		{
			_SourceGlyphs = glyphs;
		}

		protected virtual void PrepareGlyphs ()
		{
			_Glyphs = (ArrayList) _SourceGlyphs.Clone ();
			_Glyphs.Sort (new ConvertToCodeGlyphSorter ());

			_States = new ArrayList ();
			_Transitions = new ArrayList ();
			_Ports = new ArrayList ();
			foreach (IGlyph glyph in _Glyphs)
			{
				IStateGlyph state = glyph as IStateGlyph;
				if (state != null)
				{
					_States.Add (state);
				}

				ITransitionGlyph trans = glyph as ITransitionGlyph;
				if (trans != null)
				{
					_Transitions.Add (trans);
				}

				IStateTransitionPortGlyph port = glyph as IStateTransitionPortGlyph;
				if (port != null)
				{
					_Ports.Add (port);
				}
			}

			_StatesWithHistoryTransitions = new Hashtable ();
			foreach (IStateGlyph state in _States)
			{
				foreach (TransitionInfo info in GetTransitionList (state))
				{
					if (info.Transition.TransitionType == TransitionType.History)
					{
						throw new NotImplementedException ("History not yet implemented");
					}
					if (info.Transition.TransitionType == TransitionType.DeepHistory)
					{
						IStateGlyph historyKeeper = info.ToStateGlyph;
						if (!_StatesWithHistoryTransitions.Contains (historyKeeper))
						{
							_StatesWithHistoryTransitions.Add (historyKeeper, info);
						}
					}
				}
			}
		}

		protected bool ModelHasDeepHistoryTransition ()
		{
			return _StatesWithHistoryTransitions.Count > 0;
		}

		protected bool StateOrParentHasDeepHistoryTransition (IStateGlyph state)
		{
			while (state != null)
			{
				if (_StatesWithHistoryTransitions.Contains (state))
				{
					return true;
				}
				state = state.Parent as IStateGlyph;
			}
			return false;
		}

        protected bool StateHasAsAnyParent (IStateGlyph state, IStateGlyph possibleParentState)
        {
            while (state != null)
            {
                if (state.Parent == possibleParentState)
                {
                    return true;
                }
                state = state.Parent as IStateGlyph;
            }
            return false;
        }

		protected ArrayList GetStateOrParentsThatHasDeepHistoryTransition (IStateGlyph state)
		{
			ArrayList list = new ArrayList ();
			while (state != null)
			{
				if (_StatesWithHistoryTransitions.Contains (state))
				{
					list.Add (state);
				}
				state = state.Parent as IStateGlyph;
			}
			return list;
		}

		public class TransitionInfo 
		{
			public ITransitionGlyph Transition; 
			public string FromStateName; 
			public string ToStateName;

			public IStateGlyph ToStateGlyph;

			protected string ToNameString (string name)
			{
				if (name == null || name.Trim () == "")
				{
					name = "";
				} 
				else 
				{
					name = "_" + name.Trim ();
				}
				return name;
			}

			public string TransitionVarName 
			{ 
				get 
				{
					string name = ToNameString (Transition.Name);
					string eventName = ToNameString (Transition.Event);

					return string.Format ("s_trans{0}{1}_{2}_2_{3}", name, eventName, FromStateName, ToStateName);
				}
			}

			public TransitionInfo (ITransitionGlyph trans, string fromStateName, string toStateName, IStateGlyph toStateGlyph)
			{
				Transition = trans;
				FromStateName = fromStateName;
				ToStateName = toStateName;
				ToStateGlyph = toStateGlyph;
			}
		}

		protected bool IsNotEmptyString (string text)
		{
			return text != null && text.Trim () != "";
		}

		protected ArrayList GetTransitionList (IStateGlyph state)
		{
			ArrayList transitionList = new ArrayList ();
			GetTransitionList (transitionList, state);
			return transitionList;
		}

		protected void GetTransitionList (ArrayList transitionList, IStateGlyph state)
		{
			// now find transitions
			foreach (IGlyph child in state.Children)
			{
				ITransitionContactPointGlyph transContactPoint = child as ITransitionContactPointGlyph;
				if (transContactPoint != null)
				{
					ITransitionGlyph trans = transContactPoint.Owner as ITransitionGlyph;

					if (transContactPoint.WhichEnd == TransitionContactEnd.From)
					{ 
						// this is a from transition

						ITransitionContactPointGlyph transTo = null;
						foreach (ITransitionContactPointGlyph contactPoint in trans.ContactPoints)
						{
							if (contactPoint.WhichEnd == TransitionContactEnd.To)
							{
								transTo = contactPoint;
								break;
							}
						}
						System.Diagnostics.Debug.Assert (transTo != null, "Transition To Contact Point not found");

						IStateGlyph toStateGlyph = transTo.Parent as IStateGlyph;
						string toStateName = "TRANSITION_TOSTATE_NOT_SET";
						if (toStateGlyph != null)
						{
							if (IsNotEmptyString (toStateGlyph.Name))
							{
								toStateName = StateNameFrom (toStateGlyph);
							} 
							else 
							{
								toStateName = "TRANSITION_TOSTATE_SET_BUT_STATE_NOT_NAMED";
							}
						}

						TransitionInfo info = new TransitionInfo (trans, StateNameFrom (state), toStateName, toStateGlyph);
						transitionList.Add (info);
					}
				}
			}
		}

		#region Level and WriteLine
		int _LevelCounter = 0;
		string _Level = "";
		protected string Level { get { return _Level; } }
		protected void Inc ()
		{
			_LevelCounter++;
			ReInitLevel ();
		}

		protected void Dec ()
		{
			_LevelCounter--;
			System.Diagnostics.Debug.Assert (_LevelCounter >= 0);
			ReInitLevel ();
		}

		protected void ReInitLevel ()
		{
			_Level = "".PadRight (_LevelCounter, '\t');
		}

		protected virtual void WriteLine (string fmt, params object[] args)
		{
			if (args == null || args.Length == 0)
			{
				//_Builder.Append (_Level + fmt + "\n");
			} 
			else 
			{
				//_Builder.AppendFormat (_Level + fmt + "\n", args);
			}
		}
		#endregion

		protected string StateNameFrom (IStateGlyph state)
		{
			string name = state.FullyQualifiedStateName;
			name = name.Replace (".", "_");
			return name;
		}

		protected IStateGlyph OuterMostState ()
		{
			foreach (IStateGlyph state in _States)
			{
				if (state.Parent == null && state.IsStartState)
				{
					return state;
				}
			}
			return null;
		}

		protected string OuterMostStateName ()
		{
			IStateGlyph state = OuterMostState ();
			if (state != null)
			{
				return StateNameFrom (state);
			}
			return "NO_STATE_AT_OUTER_LEVEL_MARKED_AS_START_STATE";
		}

		protected virtual string[] ParseActionForSignals (string incomingAction)
		{
			return new string[] {}; 
		}

		protected ArrayList GetUniqueTransitionEvents (bool qualifiedEvent) 
		{
			Hashtable uniqueEventNames = new Hashtable ();
			ArrayList list = new ArrayList ();
			foreach (ITransitionGlyph trans in _Transitions)
			{
				System.Collections.Specialized.StringCollection eventNames = new System.Collections.Specialized.StringCollection ();
				
				string eventNameOnly = null;
				if (qualifiedEvent) 
				{
					eventNameOnly = trans.QualifiedEvent;
				} 
				else 
				{
					eventNameOnly = trans.EventSignal;
				}

				eventNames.Add (eventNameOnly);

				if (IsNotEmptyString (trans.Action))
				{
					string[] signals = ParseActionForSignals (trans.Action);
					eventNames.AddRange (signals);
				}

				foreach (string eventName in eventNames)
				{
					if (!uniqueEventNames.Contains (eventName))
					{ 
						list.Add (eventName);
						uniqueEventNames.Add (eventName, "");
					}
				}
			}
			list.Sort ();
			return list;
		}

		protected class TransitionEventAndTypeComparer : IComparer
		{
			#region IComparer Members

			protected bool IsNotEmptyString (string value)
			{
				if (value == null || value.Trim () == "")
				{
					return false;
				}
				return true;
			}

			protected string ValidString (string value)
			{
				return IsNotEmptyString (value) ? value : "";
			}

			public int Compare(object x, object y)
			{
				if (x == y)
				{
					return 0;
				}

				ITransitionGlyph xTrans = x as ITransitionGlyph;
				ITransitionGlyph yTrans = y as ITransitionGlyph;
				int compName = ValidString (xTrans.EventSignal).CompareTo (ValidString (yTrans.EventSignal));
				if (compName == 0)
				{
					return ValidString (xTrans.EventType).CompareTo (ValidString (yTrans.EventType));
				}
				return compName;
			}

			#endregion
		}

		protected ArrayList GetUniqueTransitionsByEventAndEventType () 
		{
			Hashtable uniqueEventNames = new Hashtable ();
			ArrayList list = new ArrayList ();
			foreach (ITransitionGlyph trans in _Transitions)
			{
				string eventName = string.Format ("{0}/{1}", trans.Event, trans.EventType);
				if (!uniqueEventNames.Contains (eventName))
				{ 
					list.Add (trans);
					uniqueEventNames.Add (eventName, "");
				}
			}
			list.Sort (new TransitionEventAndTypeComparer ());
			return list;
		}

		protected class TransitionOrderPrioritySorter : IComparer
		{
			protected bool IsNotEmptyString (string text)
			{
				return text != null && text.Trim () != "";
			}

			protected string ReplaceEmpty (string sv, string def)
			{
				if (IsNotEmptyString (sv))
				{
					return sv;
				} 
				else 
				{
					return def;
				}
			}

			#region IComparer Members

			public int Compare(object x, object y)
			{
				if (x == y) return 0;

				TransitionInfo xinfo = x as TransitionInfo;
				TransitionInfo yinfo = y as TransitionInfo;
			    
			    string xSignal = ReplaceEmpty (xinfo.Transition.EventSignal, "");
                string ySignal = ReplaceEmpty (yinfo.Transition.EventSignal, "");

				int compGuard =  xSignal.CompareTo (ySignal);
                if (compGuard == 0)
                {
                    string maxString = "ZZZZZZZZZZZZZZZZZZZZZZZ";
                    string xguard =  ReplaceEmpty (xinfo.Transition.GuardCondition, maxString);
                    string yguard =  ReplaceEmpty (yinfo.Transition.GuardCondition, maxString);

                    // if one of these is the else transition
                    if (xguard == maxString || yguard == maxString)
                    {
                        compGuard =  xguard.CompareTo (yguard);                        
                    }
                    
                    if (compGuard == 0)
                    {
                        compGuard = xinfo.Transition.EvaluationOrderPriority.CompareTo (yinfo.Transition.EvaluationOrderPriority);
                        if (compGuard == 0)
                        {
                            xguard = ReplaceEmpty (xinfo.Transition.EventSource, "") + "." + xguard;
                            yguard = ReplaceEmpty (xinfo.Transition.EventSource, "") + "." + yguard;				        
                            compGuard =  xguard.CompareTo (yguard);
                        }
                    }
                }

				return compGuard;
			}

			#endregion
		}


		protected ArrayList GroupTransitionsByEvent (ArrayList transitionList)
		{
			ArrayList groupedTransitions = new ArrayList ();
			SortedList groups = new SortedList ();
			int transCount = 0;
			foreach (TransitionInfo info in transitionList)
			{
				string eventName = IsNotEmptyString (info.Transition.Event) ? info.Transition.Event : transCount++.ToString ();
				if (!groups.Contains (eventName))
				{
					groups.Add (eventName, new ArrayList ());
				}
				ArrayList list = (ArrayList) groups [eventName];
				list.Add (info);
			}
			foreach (DictionaryEntry de in groups)
			{
				ArrayList list = (ArrayList) de.Value;
				list.Sort (new TransitionOrderPrioritySorter ());
				groupedTransitions.Add (list);
			}
			return groupedTransitions;
		}

		protected bool StateHasAfterTransitions (IStateGlyph state)
		{
			foreach (TransitionInfo transInfo in GetTransitionList (state))
			{
				if (IsNotEmptyString (transInfo.Transition.TimeOutExpression))
				{
					return true;
				}
			}
			return false;
		}

		protected ArrayList GetStateAfterTransitions (IStateGlyph state)
		{
			ArrayList list = new ArrayList ();
			foreach (TransitionInfo transInfo in GetTransitionList (state))
			{
				if (IsNotEmptyString (transInfo.Transition.TimeOutExpression))
				{
					list.Add (transInfo.Transition);
				}
			}
			return list;
		}

		protected bool HasGuardCondition (ITransitionGlyph trans)
		{
			return IsNotEmptyString (trans.GuardCondition);
		}

	}
}
