using System;
using System.Collections;
using System.Text;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ConvertToCode.
	/// </summary>
	public class ConvertToCode : ConvertToCodeBase
	{
		DiagramModel _Model;
		StateMachineHeader _Header;
		StringBuilder _Builder;

		string _DefaultBaseStateMachine;
		string _NameSpace;
		string _UsingNameSpaces;
		string _ClassName;
		string _DerivedClassName;
		string _SignalClassName;
		string _QualifiedSignalClassName;
		string _SignalTypeCast;
		Hashtable _CodeBlocks;

		bool _IsDerivedMachine;
		bool _UsesAtMarket = false;
		bool _WriteStateCodeBlocks = false;

		bool _Instrument = true;

		bool _SignalAsEnum;

		public ConvertToCode (DiagramModel model, bool signalAsEnum)
			: base (model.GetGlyphsList ())
		{
			_Model = model;
			_Header = model.Header;
			_SignalAsEnum = signalAsEnum;
		}

		protected void Write (string fmt, params object[] args)
		{
			if (args == null || args.Length == 0)
			{
				_Builder.Append (fmt);
			} 
			else 
			{
				_Builder.AppendFormat (fmt, args);
			}
		}

		protected override void WriteLine (string fmt, params object[] args)
		{
			if (args == null || args.Length == 0)
			{
				_Builder.Append (Level + fmt + "\n");
			} 
			else 
			{
				_Builder.AppendFormat (Level + fmt + "\n", args);
			}
		}


		protected void WriteSummary ()
		{
			WriteLine ("// SM: {0}", _Header.Name);

			foreach (IGlyph glyph in _Glyphs)
			{
				IStateGlyph state = glyph as IStateGlyph;
				if (state != null)
				{
					string overrideOrNormal = state.IsOverriding ? " - overriding" : "";
					WriteLine ("// State: {0}{1}", state.FullyQualifiedStateName, overrideOrNormal);
				}
				ITransitionGlyph trans = glyph as ITransitionGlyph;
				if (trans != null)
				{
					string text = NormalisedTransitionDisplayText (trans);
					WriteLine ("// Transition: {0}", text);
				}
			}
		}

		protected void WriteStateDefaults (IStateGlyph state)
		{
			IStateGlyph initState = null;
			foreach (IGlyph child in state.Children)
			{
				IStateGlyph childState = child as IStateGlyph;
				if (childState != null && childState.IsStartState)
				{
					System.Diagnostics.Debug.Assert (initState == null, "More than one child marked as StartState: " + state.Name);
					initState = childState;
				}
			}

			if (initState != null)
			{
				WriteLine ("case {0}QSignals.Init: {{", _SignalTypeCast);
				Inc ();
				WriteLog (StateLogType.Init, state, initState);
				WriteStateCodeBlock (state, "Init");
				WriteLine ("InitializeState (s_{0});", StateNameFrom (initState));
				Dec ();
				WriteLine ("} return null;");
			} 


			if (IsNotEmptyString (state.DoAction))
			{
				throw new NotSupportedException ("DoAction not supported. Found in state: " + StateNameFrom (state));
			}

            bool isFinalState = state.IsFinalState;
			bool hasEntryAction = IsNotEmptyString (state.EntryAction);
			bool stateMustKeepHistory = StateOrParentHasDeepHistoryTransition (state);
			bool hasAfterTransitions = StateHasAfterTransitions (state);
			if (hasEntryAction || isFinalState || stateMustKeepHistory || hasAfterTransitions)
			{
				WriteLine ("case {0}QSignals.Entry: {{", _SignalTypeCast);
				Inc ();
				WriteLog (StateLogType.Entry, state);
				WriteStateCodeBlock (state, "Entry");
				if (stateMustKeepHistory)
				{
					ArrayList historyKeepers = GetStateOrParentsThatHasDeepHistoryTransition (state);
					foreach (IStateGlyph historyKeeper in historyKeepers)
					{
						WriteLine ("_{0}_DeepHistory = s_{1};", StateNameFrom (historyKeeper), StateNameFrom (state));
					}
				}
				if (hasEntryAction)
				{
					WriteLine ("{0};", ParseAction (state.EntryAction));
				}
				if (hasAfterTransitions)
				{
					ArrayList afterTransitions = GetStateAfterTransitions (state);
					foreach (ITransitionGlyph trans in afterTransitions)
					{
						if (trans.IsInnerTransition)
						{
							throw new NotSupportedException ("After Timeout transitions are not supported for InnerTransitions");
						}
						string timeOutExpression = trans.TimeOutExpression.Trim ();
						if (timeOutExpression.IndexOf (" ") == -1)
						{
							// this is the original timeout request
							WriteLine ("SetTimeOut (\"{0}_{1}_{2}\", TimeSpan.FromSeconds ({3}), new QEvent (\"{4}\"));", StateNameFrom (state), trans.Name, trans.Event, trans.TimeOutExpression, trans.QualifiedEvent);
						} 
						else 
						{
							string[] strList = timeOutExpression.Split (' ');
							string timeOut = strList [strList.Length - 1].Trim ();
							string flag = "Single";
							if (timeOutExpression.StartsWith ("every"))
							{
								flag = "Repeat";
							}
							if (timeOutExpression.StartsWith ("at"))
							{
								flag = "Single";
								WriteLine ("SetTimeOut (\"{0}_{1}_{2}\", {3}, new QEvent (\"{4}\"), TimeOutType.{5});", StateNameFrom (state), trans.Name, trans.Event, timeOut, trans.QualifiedEvent, flag);
							} 
							else 
							{
								WriteLine ("SetTimeOut (\"{0}_{1}_{2}\", TimeSpan.FromSeconds ({3}), new QEvent (\"{4}\"), TimeOutType.{5});", StateNameFrom (state), trans.Name, trans.Event, timeOut, trans.QualifiedEvent, flag);
							}
						}
					}
				}
                if (isFinalState)
                {
                    WriteLine ("DoFinalStateReached (this, s_{0});", StateNameFrom (state));    
                }
                Dec ();
				WriteLine ("} return null;");
			} 
			else if (!state.IsOverriding)
			{
				WriteLine ("case {0}QSignals.Entry: {{", _SignalTypeCast);
				Inc ();
				WriteLog (StateLogType.Entry, state);
				WriteStateCodeBlock (state, "Entry");
				Dec ();
				WriteLine ("} return null;");
			}

			if (hasAfterTransitions || IsNotEmptyString (state.ExitAction))
			{
				WriteLine ("case {0}QSignals.Exit: {{", _SignalTypeCast);
				Inc ();
				if (hasAfterTransitions)
				{
					ArrayList afterTransitions = GetStateAfterTransitions (state);
					foreach (ITransitionGlyph trans in afterTransitions)
					{
						WriteLine ("ClearTimeOut (\"{0}_{1}_{2}\");", StateNameFrom (state), trans.Name, trans.Event);
					}
				}
				WriteStateCodeBlock (state, "Exit");
				if (IsNotEmptyString (state.ExitAction))
				{
					WriteLine ("{0};", ParseAction (state.ExitAction));
				}
				WriteLog (StateLogType.Exit, state);
				Dec ();
				WriteLine ("} return null;");
			} 
			else if (!state.IsOverriding)
			{
				WriteLine ("case {0}QSignals.Exit: {{", _SignalTypeCast);
				Inc ();
				WriteStateCodeBlock (state, "Exit");
				WriteLog (StateLogType.Exit, state);
				Dec ();
				WriteLine ("} return null;");
			}
		}

		void ParseTime (string action, out string timeOut, out string eventName)
		{
			action = action.Trim ();
			action = action.Replace ("tm(", "");
			if (action.EndsWith (")"))
			{
				action = action.Remove (action.Length - 1, 1);
			}
			string[] parameters = action.Split (',');
			timeOut = parameters [0].Trim ();
			eventName = parameters [1].Trim ();
		}

		protected enum StateLogType { Init, Entry, Exit, EventTransition, EventInternalTransition, EventBeforeTransition, EventBeforeInternalTransition }

		protected bool CanInstrument (IStateGlyph state)
		{
			return _Instrument && state.DoNotInstrument == false;
		}

		protected void WriteLog (StateLogType logType, IStateGlyph state, IStateGlyph initState)
		{
			if (CanInstrument (state))
			{
				WriteLine ("LogStateEvent (StateLogType.{0}, s_{1}, s_{2});", logType, StateNameFrom (state), StateNameFrom (initState));
			}
		}

		protected string NormalisedActionDisplayText (string action)
		{
			string text = action;
			text = text.Replace ("\"", "\\\"");
			text = text.Replace ("\r", "; ");
			text = text.Replace ("\n", "; ");
			text = text.Replace ("\r\n", "; ");
			while (text.IndexOf (";;") != -1)
			{
				text = text.Replace (";;", ";");
			}
			text = text.Trim ();
			if (text == "")
			{
				text = null;
			}
			return text;
		}


		protected void WriteLog (StateLogType logType, IStateGlyph state)
		{
			if (CanInstrument (state))
			{
				string action = null;
				switch (logType)
				{
					case StateLogType.Entry:
					{
						action = NormalisedActionDisplayText (state.EntryAction);
					} break;
					case StateLogType.Exit: 
					{
						action = NormalisedActionDisplayText (state.ExitAction);
					} break;
				}
				if (action == null)
				{
					WriteLine ("LogStateEvent (StateLogType.{0}, s_{1});", logType, StateNameFrom (state));
				} 
				else 
				{
					WriteLine ("LogStateEvent (StateLogType.{0}, s_{1}, \"{2}\");", logType, StateNameFrom (state), action);
				}
			}
		}

		protected string NormalisedTransitionDisplayText (ITransitionGlyph trans)
		{
			string text = trans.DisplayText ();
			text = text.Replace ("\"", "\\\"");
			text = text.Replace ("\r", "; ");
			text = text.Replace ("\n", "; ");
			text = text.Replace ("\r\n", "; ");
			while (text.IndexOf (";;") != -1)
			{
				text = text.Replace (";;", ";");
			}
			return text;
		}

		protected string NormalisedTransitionDisplayText (TransitionInfo transitionInfo)
		{
			return NormalisedTransitionDisplayText (transitionInfo.Transition);
		}

		protected void WriteLog (StateLogType logType, IStateGlyph state, TransitionInfo transitionInfo)
		{
            WriteLog(logType, state, transitionInfo, "s");
		}

        protected void WriteLog (StateLogType logType, IStateGlyph state, TransitionInfo transitionInfo, string transitionFieldNamePrefix)
        {
            if (CanInstrument (state))
            {
                string text = NormalisedTransitionDisplayText (transitionInfo);
                WriteLine ("LogStateEvent (StateLogType.{0}, s_{1}, {5}_{2}, \"{3}\", \"{4}\");", logType, StateNameFrom (state), transitionInfo.ToStateName, transitionInfo.Transition.QualifiedEvent, text, transitionFieldNamePrefix);
            }
        }

		protected class SignalActionParserForSignals : SignalActionParser
		{
			ArrayList _Signals;
			public SignalActionParserForSignals (ArrayList signals)
			{
				_Signals = signals;
			}

			protected override void DoPortSignalToken (string port, string signalClass, string signal, string args)
			{
				_Signals.Add (signal.Trim ());
			}
		}

		void ParseSignals (ArrayList signals, string action)
		{
			SignalActionParser parser = new SignalActionParserForSignals (signals);
			parser.Parse (_SignalClassName, action);
		}

		protected override string[] ParseActionForSignals (string incomingAction)
		{
			ArrayList signals = new ArrayList ();

			if (IsNotEmptyString (incomingAction))
			{
				incomingAction = incomingAction.Trim ();

				string[] actions = incomingAction.Split (';');
				
				foreach (string action in actions)
				{
					if (action.Trim () == "")
					{
						continue;
					}

					if (action.StartsWith ("^"))
					{
						ParseSignals (signals, action);
					} 
				}
			}
			return (string[]) signals.ToArray (typeof (string));
		}

		string ParseSignalAction (string action)
		{
			SignalActionParser parser = new SignalActionParser ();
			return parser.Parse (_SignalClassName, action);
		}

		string ParseAction (string incomingAction)
		{
			StringBuilder actionBuilder = new StringBuilder ();
			if (IsNotEmptyString (incomingAction))
			{
				incomingAction = incomingAction.Trim ();

				incomingAction = incomingAction.Replace ("\r\n", ";");
				incomingAction = incomingAction.Replace ("\r", ";");
				incomingAction = incomingAction.Replace ("\n", ";");
				string[] actions = incomingAction.Split (';');
				
				int round = 0;
				foreach (string action in actions)
				{
					if (action.Trim () == "")
					{
						continue;
					}

					if (round > 0)
					{
						actionBuilder.Append (";\n" + Level);
					}

					round++;

					if (action.StartsWith ("tm("))
					{
						string timeOut;
						string eventName;
						ParseTime (action, out timeOut, out eventName);
						string replaceAction = string.Format ("SetTimeOut (TimeSpan.FromSeconds ({0}), new QEvent ({1}{2}.{3})", timeOut, _SignalTypeCast, _SignalClassName, eventName);
						actionBuilder.Append (replaceAction.Trim ());
					}
					else if (action.StartsWith ("^"))
					{
						string replaceAction = ParseSignalAction (action);
						actionBuilder.Append (replaceAction.Trim ());
					} 
					else 
					{
						actionBuilder.Append (action.Trim ());
					}
				}

			}
			return actionBuilder.ToString ().Trim ();
		}

		protected class TransitionVarComparer : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				if (x == y) return 0;
				TransitionInfo xinfo = x as TransitionInfo;
				TransitionInfo yinfo = y as TransitionInfo;

				return xinfo.TransitionVarName.CompareTo (yinfo.TransitionVarName);
			}

			#endregion
		}

		protected void WriteStaticTransitionChains (IStateGlyph state, ArrayList transitionList)
		{
			Hashtable writtenTrans = new Hashtable ();
			transitionList.Sort (new TransitionVarComparer ());
			foreach (TransitionInfo transInfo in transitionList)
			{
				ITransitionGlyph trans = transInfo.Transition;
				if (!trans.IsProperInnerTransition ())
				{
					string sv = transInfo.TransitionVarName;
					if (!writtenTrans.Contains (sv))
					{
						WriteLine ("protected static int {0} = s_TransitionChainStore.GetOpenSlot ();", sv);
						writtenTrans.Add (sv, sv);
					}
				}
			}
		}

		protected void CheckTransitions_CatchDuplicateEvents (IStateGlyph state, ArrayList groupedTransitionList)
		{
			Hashtable catcher = new Hashtable ();
			foreach (TransitionInfo transInfo in groupedTransitionList)
			{
				ITransitionGlyph trans = transInfo.Transition;

				string eventText = trans.CompleteEventText (false, false);
				if (catcher.Contains (eventText))
				{
					string msg = string.Format ("More than one transition from the same State: {0} have the same event {1}", StateNameFrom (state), eventText);
					throw new ArgumentException (msg);
				}
				catcher.Add (eventText, eventText);
			}
		}

		protected void CheckTransitions_DuplicateTransitionsNoGuard (IStateGlyph state, ArrayList groupedTransitionList)
		{
			// not really necessary - but supplies extra info.
			bool guardLessTransFound = false;
			foreach (TransitionInfo transInfo in groupedTransitionList)
			{
				ITransitionGlyph trans = transInfo.Transition;

				if (IsNotEmptyString (trans.GuardCondition) == false)
				{
					if (guardLessTransFound)
					{
						string msg = string.Format ("Two transitions from the same State: {0} have the same event {1} without guard conditions", StateNameFrom (state), trans.QualifiedEvent);
						throw new ArgumentException (msg);
					}
					guardLessTransFound = true;
				}
			}
		}

		protected void CheckTransitions_CheckUniqueNames (IStateGlyph state, ArrayList groupedTransitionList)
		{
			Hashtable catcher = new Hashtable ();
			foreach (TransitionInfo transInfo in groupedTransitionList)
			{
				ITransitionGlyph trans = transInfo.Transition;

				string text = trans.Name;
				if (IsNotEmptyString (text))
				{
					if (catcher.Contains (text))
					{
						string msg = string.Format ("More than one transition has same name {0}. Last one found in state {1} event {2}", text, StateNameFrom (state), trans.DisplayText ());
						throw new ArgumentException (msg);
					}
					catcher.Add (text, text);
				}
			}
		}

		protected void CheckTransitions (IStateGlyph state, ArrayList groupedTransitionList)
		{
			CheckTransitions_DuplicateTransitionsNoGuard (state, groupedTransitionList);
			CheckTransitions_CatchDuplicateEvents (state, groupedTransitionList);
			CheckTransitions_CheckUniqueNames (state, groupedTransitionList);
		}


		protected void WriteTransition (IStateGlyph state, ArrayList groupedTransitionList)
		{
			ITransitionGlyph trans = (groupedTransitionList [0] as TransitionInfo).Transition;
			WriteLine ("case {0}{1}.{2}: {{", _SignalTypeCast, _QualifiedSignalClassName, trans.Event);
			Inc ();
			int round = 0;
			string breakString = "break;"; // if no non-empty guard condition so execution could fall through all tests - so break at end of case block.
			CheckTransitions (state, groupedTransitionList);
			foreach (TransitionInfo transInfo in groupedTransitionList)
			{
				trans = transInfo.Transition;

                bool transitionHasAction = IsNotEmptyString (trans.Action);
                bool transitionHasGuard = HasGuardCondition (trans);

				if (transitionHasGuard == false)
				{
					breakString = ""; // empty guard condition and empty source - so conditionless handler - no need for break as no fall-through.
				}

				if (transitionHasGuard)
				{
					string elseOp = round > 0 ? "else" : "";
					string condition = trans.GuardCondition;
					WriteLine ("{1} if ({0}) {{", condition, elseOp);
					Inc ();
				}
				else if (round > 0)
				{
					WriteLine ("else {");
					Inc ();
				}

                switch (trans.TransitionType)
                {
                    case TransitionType.History: 
                    {
                        throw new NotSupportedException ("History transitions are not yet supported");
                    }
                    case TransitionType.DeepHistory: 
                    {
                        WriteLine ("QState toState_{0} = _{0}_DeepHistory == null ? s_{0} : _{0}_DeepHistory;", transInfo.ToStateName);
                    } break;
                }

                /*
                if (trans.IsProperInnerTransition ())
                {
                    WriteLog (StateLogType.EventBeforeInternalTransition, state, transInfo);
                } 
                else 
                {
                    switch (trans.TransitionType)
                    {
                        case TransitionType.History: 
                        {
                            throw new NotSupportedException ("History transitions are not yet supported");
                        }
                        case TransitionType.DeepHistory: 
                        {
                            WriteLog (StateLogType.EventBeforeTransition, state, transInfo, "toState");
                        } break;
                        default:
                        {
                            WriteLog (StateLogType.EventBeforeTransition, state, transInfo);                            
                        } break;
                    }
                }
                */

				string codeBlockName = _SignalClassName + "." + trans.QualifiedEvent;
				if (transitionHasGuard)
				{
					if (IsNotEmptyString (trans.Name) == false)
					{
						string msg = string.Format ("State {0} - transition with guard condition must have a name: {1}", StateNameFrom (state), trans.DisplayText ());
						throw new ArgumentException (msg);
					}
					codeBlockName += ":" + trans.Name;
				}
				WriteStateCodeBlock (state, codeBlockName);

				if (transitionHasAction)
				{
					string action = ParseAction (trans.Action);
					WriteLine ("{0};", action);
				}

				if (!trans.IsProperInnerTransition ())
				{
					switch (trans.TransitionType)
					{
						case TransitionType.History: 
						{
							throw new NotSupportedException ("History transitions are not yet supported");
						}
						case TransitionType.DeepHistory: 
						{
                            WriteLog (StateLogType.EventTransition, state, transInfo, "toState");
							WriteLine ("TransitionTo (toState_{0});", transInfo.ToStateName);
						} break;
						default: 
						{
                            WriteLog (StateLogType.EventTransition, state, transInfo);
                            WriteLine ("TransitionTo (s_{0}, {1});", transInfo.ToStateName, transInfo.TransitionVarName);
						} break;
					}
				}

                if (trans.IsProperInnerTransition ())
                {
                    WriteLog (StateLogType.EventInternalTransition, state, transInfo);
                } 

				WriteLine ("return null;");

				if (transitionHasGuard || round > 0)
				{
					Dec ();
					WriteLine ("}");
				} 

				round++;
			}
			Dec ();
			WriteLine ("}} {0} // {1}", breakString, trans.QualifiedEvent); 
		}

		protected void WriteState (IStateGlyph state)
		{
			ArrayList transitionList = GetTransitionList (state);
			Inc ();
			WriteLine ("");
			WriteLine ("#region State {0}", StateNameFrom (state));

			WriteStaticTransitionChains (state, transitionList);

			WriteLine ("[StateMethod (\"{0}\")]", StateNameFrom (state));
			foreach (string stateCommandName in state.StateCommands)
			{
				if (IsNotEmptyString (stateCommandName))
				{
					WriteLine ("[StateCommand (\"{0}\")]", stateCommandName);
				}
			}
			string overrideOrVirtual = state.IsOverriding ? "override" : "virtual";
			WriteLine ("protected {0} QState S_{1} (IQEvent ev){{", overrideOrVirtual, StateNameFrom (state));
			Inc ();
			WriteLine ("switch (ev.QSignal){");
			WriteStateDefaults (state);

			// now find transitions
			ArrayList groupedTransitions = GroupTransitionsByEvent (transitionList);
			foreach (ArrayList groupedTransitionList in groupedTransitions)
			{
				WriteTransition (state, groupedTransitionList);
			}
			WriteLine ("} // switch");

			// get return parent state
			string parentName = "TopState";
			IGlyph parentGlyph = state.Parent;
			if (parentGlyph != null)
			{
				IStateGlyph parentState = parentGlyph as IStateGlyph;
				parentName = "s_" + StateNameFrom (parentState);
			}

			WriteLine ("");
			if (state.IsOverriding)
			{
				WriteLine ("return base.S_{0} (ev);", StateNameFrom (state));
			} 
			else 
			{
				WriteLine ("return {0};", parentName);
			}

			Dec ();
			WriteLine ("}} // S_{0}", StateNameFrom (state));
			WriteLine ("#endregion");
			Dec ();
		}

        protected void WriteFinalStateFunction ()
        {
            Inc ();
            WriteLine ("");
            WriteLine ("#region IsFinalState");
            WriteLine ("public override bool IsFinalState (QState state){");
            Inc ();

            WriteLine ("return false");
            foreach (IStateGlyph state in _States)
            {
                if (state.IsFinalState)
                {
                    WriteLine ("|| state == s_{0}", StateNameFrom (state));
                }
            }    
            WriteLine (";");

            Dec ();
            WriteLine ("}");
            WriteLine ("#endregion // IsFinalState");
            WriteLine ("");
            Dec ();
        }

		protected void WriteStates ()
		{
            WriteFinalStateFunction ();
			foreach (IStateGlyph state in _States)
			{
				WriteState (state);
				WriteLine ("");
			}
		}

		protected void WriteSMBoilerPlate ()
		{
			Inc ();
			WriteLine ("#region Boiler plate static stuff");
			WriteLine ("protected static new TransitionChainStore s_TransitionChainStore = ");
			WriteLine ("	new TransitionChainStore(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);");
            WriteLine ("static object _MyStaticInstance;");
			WriteLine ("static {0} ()", _ClassName);
			WriteLine ("{");
			WriteLine ("	s_TransitionChainStore.ShrinkToActualSize();");
            WriteLine ("    _MyStaticInstance = new {0}();", _ClassName);
            WriteLine ("}");
			WriteLine ("protected override TransitionChainStore TransChainStore");
			WriteLine ("{");
			WriteLine ("	get { return s_TransitionChainStore; }");
			WriteLine ("}");
			WriteLine ("#endregion");
			WriteLine ("");
			
			if (!_IsDerivedMachine)
			{
				WriteLine ("protected override void InitializeStateMachine()");
				WriteLine ("{");
				Inc ();
				WriteLine ("InitializeState(s_{0});", OuterMostStateName ());
				WriteCodeBlock ("InitialiseSM");
				Dec ();
				WriteLine ("}");
			}

			Dec ();
		}

		protected void WriteFields ()
		{
			Inc ();
			WriteLine ("");
			WriteLine ("#region State Fields");
			foreach (IStateGlyph state in _States)
			{
				if (!state.IsOverriding)
				{
					WriteLine ("static protected QState s_{0};", StateNameFrom (state));
				}
			}
			WriteLine ("");
			if (ModelHasDeepHistoryTransition ())
			{
				ArrayList list = new ArrayList ();
				foreach (DictionaryEntry de in _StatesWithHistoryTransitions)
				{
					IStateGlyph state = de.Key as IStateGlyph;
					list.Add (StateNameFrom (state));
				}
				list.Sort ();
				foreach (string stateName in list)
				{
					WriteLine ("protected QState _{0}_DeepHistory;", stateName);
				}
			}
			WriteLine ("#endregion");
			Dec ();
		}

		protected void WriteHistorySerialiserDeserialiser ()
		{
			WriteHistorySerialiserDeserialiser_IO ();
			WriteHistorySerialiserDeserialiser_Memento ();
            WriteHistoryStateForRestore ();
		}

		protected void WriteHistorySerialiserDeserialiser_IO ()
		{
			if (ModelHasDeepHistoryTransition ())
			{
				ArrayList list = new ArrayList ();
				foreach (DictionaryEntry de in _StatesWithHistoryTransitions)
				{
					IStateGlyph state = de.Key as IStateGlyph;
					list.Add (StateNameFrom (state));
				}
				list.Sort ();

				Inc ();
				WriteLine ("");
				WriteLine ("#region State History Serialiser/Deserialiser");

				WriteLine ("protected override void SaveHistoryStates(ISerialisationContext context)");
				WriteLine ("{");
				Inc ();
				WriteLine ("base.SaveHistoryStates (context);");

				foreach (string stateName in list)
				{
					WriteLine ("if (null == _{0}_DeepHistory)", stateName);
					WriteLine ("{");
					Inc ();
					WriteLine ("context.Formatter.Serialize (context.Stream, null);");
					Dec ();
					WriteLine ("}");
					WriteLine ("else");
					WriteLine ("{");
					Inc ();
					WriteLine ("context.Formatter.Serialize (context.Stream, _{0}_DeepHistory.Method.Name);", stateName);
					Dec ();
					WriteLine ("}");
				}

				Dec ();
				WriteLine ("}");

				WriteLine ("");
				WriteLine ("protected override void LoadHistoryStates(ISerialisationContext context)");
				WriteLine ("{");
				Inc ();
				WriteLine ("base.LoadHistoryStates (context);");

				foreach (string stateName in list)
				{
					WriteLine ("string methodName_{0} = (string) context.Formatter.Deserialize (context.Stream);", stateName);
					WriteLine ("if (null == methodName_{0})", stateName);
					WriteLine ("{");
					Inc ();
					WriteLine ("_{0}_DeepHistory = null;", stateName);
					Dec ();
					WriteLine ("}");
					WriteLine ("else");
					WriteLine ("{");
					Inc ();
					WriteLine ("_{0}_DeepHistory = (QState) Delegate.CreateDelegate (typeof (QState), this, methodName_{0});", stateName);
					Dec ();
					WriteLine ("}");
				}

				Dec ();
				WriteLine ("}");

				WriteLine ("#endregion");
				Dec ();
			}
		}

		protected void WriteHistorySerialiserDeserialiser_Memento ()
		{
			if (ModelHasDeepHistoryTransition ())
			{
				ArrayList list = new ArrayList ();
				foreach (DictionaryEntry de in _StatesWithHistoryTransitions)
				{
					IStateGlyph state = de.Key as IStateGlyph;
					list.Add (StateNameFrom (state));
				}
				list.Sort ();

				Inc ();
				WriteLine ("");
				WriteLine ("#region State History Memento");

				WriteLine ("protected override void SaveHistoryStates(ILQHsmMemento memento)");
				WriteLine ("{");
				Inc ();
				WriteLine ("base.SaveHistoryStates (memento);");

				foreach (string stateName in list)
				{
					WriteLine ("if (null == _{0}_DeepHistory)", stateName);
					WriteLine ("{");
					Inc ();
					WriteLine ("memento.AddHistoryState (\"{0}\", null);", stateName);
					Dec ();
					WriteLine ("}");
					WriteLine ("else");
					WriteLine ("{");
					Inc ();
					WriteLine ("memento.AddHistoryState (\"{0}\", _{0}_DeepHistory.Method);", stateName);
					Dec ();
					WriteLine ("}");
				}

				Dec ();
				WriteLine ("}");

				WriteLine ("");
				WriteLine ("protected override void RestoreHistoryStates(ILQHsmMemento memento)");
				WriteLine ("{");
				Inc ();
				WriteLine ("base.RestoreHistoryStates (memento);");

				foreach (string stateName in list)
				{                    
					WriteLine ("IStateMethodInfo stateInfo_{0} = memento.GetHistoryStateFor(\"{0}\");", stateName);
					WriteLine ("if (null == stateInfo_{0})", stateName);
					WriteLine ("{");
					Inc ();
					WriteLine ("_{0}_DeepHistory = null;", stateName);
					Dec ();
					WriteLine ("}");
					WriteLine ("else");
					WriteLine ("{");
					Inc ();
					WriteLine ("_{0}_DeepHistory = (QState) Delegate.CreateDelegate (typeof (QState), this, stateInfo_{0}.Method.Name);", stateName);
					Dec ();
					WriteLine ("}");
				}

				Dec ();
				WriteLine ("}");

				WriteLine ("#endregion");
				Dec ();
			}
		}


        protected void WriteHistoryStateForRestore ()
        {
            if (ModelHasDeepHistoryTransition ())
            {
                ArrayList list = new ArrayList ();
                foreach (DictionaryEntry de in _StatesWithHistoryTransitions)
                {
                    IStateGlyph state = de.Key as IStateGlyph;
                    list.Add (StateNameFrom (state));
                    
                }
                list.Sort ();

                Inc ();
                WriteLine ("");
                WriteLine ("#region Restore History State Extra");

                WriteLine ("");
                WriteLine ("public void SpecialCase_RestoreHistoryStatesRelatedToCurrentState(ILQHsmMemento memento)");
                WriteLine ("{");
                Inc ();

                bool ifSectionCompleted = false;
                foreach (IStateGlyph state in _States)
                {
                    if (!state.IsOverriding)
                    {

                        bool willSetHistoryState = false;
                        foreach (DictionaryEntry de in _StatesWithHistoryTransitions)
                        {                    
                            IStateGlyph statesWithHistoryTransitions = de.Key as IStateGlyph;

                            if(this.StateHasAsAnyParent(state, statesWithHistoryTransitions))
                            {
                                willSetHistoryState = true;
                                break;
                            }
                        }

                        if (willSetHistoryState)
                        {
                            string elseString = "";
                            if(ifSectionCompleted)
                            {
                                elseString = "else ";
                            }

                            WriteLine ("{0}if(s_{1}.Method == this.CurrentStateMethod){{", elseString, StateNameFrom (state));
                            Inc();

                            foreach (DictionaryEntry de in _StatesWithHistoryTransitions)
                            {                    
                                IStateGlyph statesWithHistoryTransitions = de.Key as IStateGlyph;

                                if(this.StateHasAsAnyParent(state, statesWithHistoryTransitions))
                                {
                                    string statesWithHistoryTransitionsName = StateNameFrom (statesWithHistoryTransitions);
                                    WriteLine ("_{0}_DeepHistory = s_{1};", statesWithHistoryTransitionsName, StateNameFrom (state));
                                }
                            }

                            Dec();
                            WriteLine ("}");

                            ifSectionCompleted = true;
                        }
                    }
                }

                Dec ();
                WriteLine ("}");

                WriteLine ("#endregion");
                Dec ();
            }
        }

        protected void WriteConstructor ()
		{
            Inc ();
            WriteLine ("#region Constructors");

            WriteLine ("public {0} (){{", _ClassName);
			Inc ();
			WriteLine ("CreateStateFields ();");
			Dec ();
			WriteLine ("}");
			WriteLine ("");

            WriteLine ("public {0} (bool createEventManager)", _ClassName);
            WriteLine ("  : base (createEventManager) {");
            Inc ();
            WriteLine ("CreateStateFields ();");
            Dec ();
            WriteLine ("}");
            WriteLine ("");

			WriteLine ("public {0} (IQEventManager eventManager)", _ClassName);
			WriteLine ("  : base (eventManager) {");
			Inc ();
			WriteLine ("CreateStateFields ();");
			Dec ();
			WriteLine ("}");
			WriteLine ("");

            WriteLine ("public {0} (string id, string groupId)", _ClassName);
            WriteLine ("  : base (id, groupId) {");
            Inc ();
            WriteLine ("CreateStateFields ();");
            Dec ();
            WriteLine ("}");
            WriteLine ("");

            WriteLine ("public {0} (string id, IQEventManager eventManager)", _ClassName);
            WriteLine ("  : base (id, eventManager) {");
            Inc ();
            WriteLine ("CreateStateFields ();");
            Dec ();
            WriteLine ("}");
            WriteLine ("");

            WriteLine ("public {0} (string id, string groupId, IQEventManager eventManager)", _ClassName);
            WriteLine ("  : base (id, groupId, eventManager) {");
            Inc ();
            WriteLine ("CreateStateFields ();");
            Dec ();
            WriteLine ("}");
            WriteLine ("");

            WriteLine ("public {0} (string id, IQHsmLifeCycleManager lifeCycleManager)", _ClassName);
            WriteLine ("  : base (id, lifeCycleManager) {");
            Inc ();
            WriteLine ("CreateStateFields ();");
            Dec ();
            WriteLine ("}");
            WriteLine ("");

            WriteLine ("public {0} (string id, string groupId, IQHsmLifeCycleManager lifeCycleManager)", _ClassName);
            WriteLine ("  : base (id, groupId, lifeCycleManager) {");
            Inc ();
            WriteLine ("CreateStateFields ();");
            Dec ();
            WriteLine ("}");
            WriteLine ("");

            WriteLine ("public {0} (string id, IQHsmExecutionContext executionContext)", _ClassName);
            WriteLine ("  : base (id, executionContext) {");
            Inc ();
            WriteLine ("CreateStateFields ();");
            Dec ();
            WriteLine ("}");
            WriteLine ("");

            WriteLine ("public {0} (string id, string groupId, IQHsmExecutionContext executionContext)", _ClassName);
            WriteLine ("  : base (id, groupId, executionContext) {");
            Inc ();
            WriteLine ("CreateStateFields ();");
            Dec ();
            WriteLine ("}");
            WriteLine ("");

			if (_UsesAtMarket)
			{
				WriteLine ("public {0} (Peresys.Runtime.Scheduler.IScheduler timer)", _ClassName);
				WriteLine ("  : base (timer) {");
				Inc ();
				WriteLine ("CreateStateFields ();");
				Dec ();
				WriteLine ("}");
				WriteLine ("");

				WriteLine ("public {0} (IHsmEventManager eventManager)", _ClassName);
				WriteLine ("  : base (eventManager) {");
				Inc ();
				WriteLine ("CreateStateFields ();");
				Dec ();
				WriteLine ("}");
				WriteLine ("");
			}
            WriteLine ("#endregion // Constructors");

			string overrideOrVirtual = _IsDerivedMachine ? "override" : "virtual";
			WriteLine ("#region Create State Fields");
			WriteLine ("protected {0} void CreateStateFields (){{", overrideOrVirtual);
			Inc ();
            WriteLine("if(null == _MyStaticInstance){");
            Inc ();
			if (_IsDerivedMachine)
			{
				WriteLine ("base.CreateStateFields ();");
			}
			foreach (IStateGlyph state in _States)
			{
				if (!state.IsOverriding)
				{
					WriteLine ("s_{0} = new QState (S_{0});", StateNameFrom (state));
				}
			}
            Dec ();
            WriteLine ("}");
			Dec ();
			WriteLine ("}");
			WriteLine ("#endregion");
            Dec ();
        }

		protected void WriteSignalsAsEnum ()
		{
			WriteLine ("public enum {0} : int", _SignalClassName);
			WriteLine ("{");
			Inc ();
			bool first = true;
			foreach (string eventName in GetUniqueTransitionEvents (false))
			{
				if (first)
				{
					first = false;
					WriteLine ("{0} = Peresys.Runtime.Active.Components.RestoreControlSignals.UserSig,", eventName);
				} 
				else 
				{
					WriteLine ("{0},", eventName);
				}

			}
			WriteLine ("UserSig");
			Dec ();
			WriteLine ("}");
		}

		protected void WriteQualifiedSignalsAsConstString ()
		{
			WriteLine ("public class {0}", _QualifiedSignalClassName);
			WriteLine ("{");
			Inc ();
			foreach (string eventName in GetUniqueTransitionEvents (true))
			{
				string eventName2 = eventName.Replace ('.', '_');
				WriteLine ("public const string {0} = \"{1}\";", eventName2, eventName);
			}
			Dec ();
			WriteLine ("}");
		}

		protected void WriteUnqualifiedSignalsAsConstString ()
		{
			WriteLine ("public class {0}", _SignalClassName);
			WriteLine ("{");
			Inc ();
			foreach (string eventName in GetUniqueTransitionEvents (false))
			{
				WriteLine ("public const string {0} = \"{1}\";", eventName, eventName);
			}
			Dec ();
			WriteLine ("}");
		}

		protected void WriteSignalsAsConstString ()
		{
			WriteQualifiedSignalsAsConstString ();
			WriteUnqualifiedSignalsAsConstString ();
		}

		protected void WriteSignalImplementation ()
		{
			Inc ();
			WriteLine ("#region ISig{0} Members", _ClassName);
			foreach (string eventName in GetUniqueTransitionEvents (false))
			{
				WriteLine ("public void Sig{0} (object data) {{ AsyncDispatch (new QEvent ({1}.{0}, data)); }}", eventName, _SignalClassName);
			}
			WriteLine ("#endregion // ISig{0} Members", _ClassName);
			Dec ();
		}

		protected void WriteSignalInterface ()
		{
			WriteLine ("public interface ISig{0}", _ClassName);
			WriteLine ("{");
			Inc ();
			foreach (string eventName in GetUniqueTransitionEvents (false))
			{
				WriteLine ("void Sig{0} (object data);", eventName);
			}
			Dec ();
			WriteLine ("}");
		}

		protected void WriteSignals ()
		{
			if (_SignalAsEnum)
			{
				WriteSignalsAsEnum ();
			} 
			else 
			{
				WriteSignalsAsConstString ();
			}
		}

		protected void Init ()
		{
			_ClassName = _Header.Name;

			if (_UsesAtMarket) 
			{
				_DefaultBaseStateMachine = "ActiveComponentBase";
			} 
			else 
			{
				if (_Header.HasSubMachines)
				{
					_DefaultBaseStateMachine = "LQHsmWithSubMachines";
				} 
				else 
				{
					_DefaultBaseStateMachine = "LQHsm";
				}
			}

			_IsDerivedMachine = IsNotEmptyString (_Header.BaseStateMachine);
			_DerivedClassName = _IsDerivedMachine ? _Header.BaseStateMachine : _DefaultBaseStateMachine;

			_SignalTypeCast = _SignalAsEnum ? "(int)" : "";
			_SignalClassName = _ClassName + "Signals";
			_QualifiedSignalClassName = "Qualified" + _ClassName + "Signals";

			_NameSpace = IsNotEmptyString (_Header.NameSpace) ? _Header.NameSpace : "MyNS";
			_UsingNameSpaces = IsNotEmptyString (_Header.UsingNameSpaces) ? _Header.UsingNameSpaces : "";
		}

		protected void WriteBeginBlock (string name)
		{
			WriteLine ("//Begin[[{0}]]", name);
		}

		protected void WriteEndBlock (string name)
		{
			WriteLine ("//End[[{0}]]", name);
		}

		protected string GetCodeBlock (string code)
		{
			if (_CodeBlocks != null)
			{
				if (_CodeBlocks.Contains (code))
				{
					CodeBlock codeBlock = _CodeBlocks [code] as CodeBlock;
					codeBlock.Use ();
					return  codeBlock.Value;
				}
			}
			return "";
		}

		protected void ValidateCodeBlocksUsed ()
		{
			if (_CodeBlocks != null)
			{
				StringBuilder builder = new StringBuilder ();
				foreach (DictionaryEntry de in _CodeBlocks)
				{
					CodeBlock codeBlock = de.Value as CodeBlock;
					if (codeBlock.UsageCount == 0)
					{
						if (IsNotEmptyString (codeBlock.Value))
						{
							builder.AppendFormat ("CodeBlock: {0}\n\n", de.Key.ToString ());
							builder.Append (codeBlock.Value);
							builder.Append ("\n\n");
						}
					}
				}
				string unusedCodeBlocks = builder.ToString ().Trim ();
				if (IsNotEmptyString (unusedCodeBlocks))
				{
					throw new FormatException ("Unused Code Blocks\n\n" + unusedCodeBlocks);
				}
			}
		}

		protected void Clear ()
		{
			if (_CodeBlocks != null)
			{
				_CodeBlocks.Clear ();
			}
		}


		protected string ExtendCodeBlock(string code, string block, ExtendCodeBlockHandler handler)
		{
			if (handler != null)
			{
				block = handler (code, block);
			}
			return block;
		}

		protected void WriteCodeBlock(string code)
		{
			WriteCodeBlock (code, null);
		}

		protected void WriteCodeBlock(string code, ExtendCodeBlockHandler handler)
		{
			WriteBeginBlock (code);
			string block = GetCodeBlock (code);
			block = ExtendCodeBlock (code, block, handler);
			if (IsNotEmptyString (block))
			{
				block = block.Trim ();
				if (IsNotEmptyString (block))
				{
					WriteLine (block);
				}
			}
			WriteEndBlock (code);
		}

		protected void WriteStateCodeBlock (IStateGlyph state, string eventName)
		{
			string stateName = StateNameFrom (state);
			string codeBlockName = string.Format ("{0}:{1}", stateName, eventName);
			if (_WriteStateCodeBlocks)
			{
				WriteCodeBlock (codeBlockName);
			} 
			else 
			{
				string block = GetCodeBlock (codeBlockName);
				if (IsNotEmptyString (block))
				{
					throw new Exception ("Don't want to use these code blocks for now... - can put them back in later!\n\nCodeBlock: " + block);
				}
			}
		}

		protected void AnalyseActions ()
		{
			ActionAnalyser analyser = new ActionAnalyser (_Model.GetGlyphsList ());
			analyser.Analyse ();
		}

		public delegate string ExtendCodeBlockHandler (string codeBlockName, string block);

		protected virtual string ExtendClassBodyCodeBlock (string codeBlockName, string block)
		{
			return block;
		}

        protected void WriteStartOfFileCodeBlock ()
        {
            WriteLine ("//---------------------------------------------------------------------");
            WriteCodeBlock ("StartOfFileBlock");            
            WriteLine ("//---------------------------------------------------------------------");
            WriteLine ("");
        }
	    
        protected void WriteImplementsInterfacseBlock ()
        {
            WriteLine ("//---------------------------------------------------------------------");
            ExtendCodeBlockHandler handler = new ExtendCodeBlockHandler (ExtendClassBodyCodeBlock);
            WriteCodeBlock ("ImplementsInterfaces", handler);
            WriteLine ("//---------------------------------------------------------------------");
        }

		protected void WriteClassBlock ()
		{
			AnalyseActions ();

			Inc ();
			WriteLine ("");
			WriteLine ("//---------------------------------------------------------------------");
			ExtendCodeBlockHandler handler = new ExtendCodeBlockHandler (ExtendClassBodyCodeBlock);
			WriteCodeBlock ("ClassBodyCode", handler);
			WriteLine ("//---------------------------------------------------------------------");
			Dec ();
		}

		protected class PortComparer : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				if (x == y) return 0;

				IStateTransitionPortGlyph xport = x as IStateTransitionPortGlyph;
				IStateTransitionPortGlyph yport = y as IStateTransitionPortGlyph;

				return xport.Name.CompareTo (yport.Name);
			}

			#endregion
		}

		protected ArrayList SortedPorts (ArrayList ports)
		{
			ports = (ArrayList) ports.Clone ();
			ports.Sort (new PortComparer ());
			return ports;
		}

		protected void WritePorts ()
		{
			if (_Ports.Count > 0)
			{
				Inc ();
				WriteLine ("");
				WriteLine ("#region Ports");
			}
			ArrayList ports = SortedPorts (_Ports);
			foreach (IStateTransitionPortGlyph port in ports)
			{
				string portName = port.Name;
				if (port.IsMultiPort)
				{
					WriteLine ("protected IQMultiPort _{0};", portName);
					WriteLine ("public IQMultiPort {0} {{ get {{ if (_{0} == null) {{ _{0} = CreateMultiPort (\"{0}\"); }} return _{0}; }} }}", portName);
				} 
				else 
				{
					WriteLine ("protected IQPort _{0};", portName);
					WriteLine ("public IQPort {0} {{ get {{ if (_{0} == null) {{ _{0} = CreatePort (\"{0}\"); }} return _{0}; }} }}", portName);
				}
			}
			if (_Ports.Count > 0)
			{
				WriteLine ("#endregion");
				WriteLine ("");
				Dec ();
			}
		}

		protected void WriteClassTransitionAttributes ()
		{
			foreach (ITransitionGlyph trans in GetUniqueTransitionsByEventAndEventType ())
			{
				if (IsNotEmptyString (trans.EventSource))
				{
					if (IsNotEmptyString (trans.EventType))
					{
						WriteLine ("[TransitionEvent (\"{0}\", \"{1}\", typeof ({2}))]", trans.EventSignal, trans.EventSource, trans.EventType);
					} 
					else 
					{
						WriteLine ("[TransitionEvent (\"{0}\", \"{1}\")]", trans.EventSignal, trans.EventSource);
					}
				} 
				else 
				{
					if (IsNotEmptyString (trans.EventType))
					{
						WriteLine ("[TransitionEvent (\"{0}\", typeof ({1}))]", trans.EventSignal, trans.EventType);
					} 
					else 
					{
						WriteLine ("[TransitionEvent (\"{0}\")]", trans.EventSignal);
					}
				}
			}
		}

		protected void WriteClass ()
		{
			WriteLine ("[ModelInformation (@\"{0}\", \"{1}\", \"{2}\")]", _Header.ModelFileName, _Header.ModelGuid, _Header.StateMachineVersion);
			WriteClassTransitionAttributes ();
			WriteLine ("public class {0} : {1}, ISig{2}", _ClassName, _DerivedClassName, _ClassName);
            WriteImplementsInterfacseBlock ();
            WriteLine ("{");

		    WriteClassBlock ();

            WriteSMBoilerPlate ();
			WriteFields ();
			WritePorts ();
			WriteConstructor ();

			WriteHistorySerialiserDeserialiser ();

			WriteStates ();

			WriteSignalImplementation ();

			WriteLine ("}} // {0}", _ClassName);
		}

		protected void WriteOuterScope ()
		{
            WriteStartOfFileCodeBlock ();

			WriteLine ("using System;");

			string[] usingNamespaces = _UsingNameSpaces.Split (";".ToCharArray ());
			foreach (string ns in usingNamespaces)
			{
				if (ns.Trim () != "")
				{
					WriteLine ("using {0};", ns);
				}
			}

			WriteLine ("using qf4net;");

			if (_UsesAtMarket)
			{
				WriteLine ("using Peresys.Core.Agent;");
				WriteLine ("using Peresys.Runtime.Scheduler;");
				WriteLine ("using Peresys.Runtime.Active.Components;");
			}

			WriteUsingNamespacesCodeBlock ();

			WriteLine ("");

			WriteLine ("namespace {0}", _NameSpace);
			WriteLine ("{");
			Inc ();

			WriteSummary ();
			WriteClass ();
			WriteSignalInterface ();
			WriteSignals ();

			Dec ();
			WriteLine ("}");		
		}

		protected void WriteUsingNamespacesCodeBlock ()
		{
			WriteLine ("");
			WriteLine ("//---------------------------------------------------------------------");
			WriteCodeBlock ("UsingNameSpaceCodeBlock");
			WriteLine ("//---------------------------------------------------------------------");
		}

		public string Convert ()
		{
			_Builder = new StringBuilder ();
			PrepareGlyphs ();
			Init ();
			WriteOuterScope ();
			ValidateCodeBlocksUsed ();


			string text = _Builder.ToString ().Trim ();

			Clear ();

			return text;
		}

		protected string StripCodeBlockName (string line, string startsWith)
		{
			if (line.StartsWith (startsWith))
			{
				string codeBlock = line.Replace (startsWith, "");
				if (codeBlock.EndsWith ("]]"))
				{
					codeBlock = codeBlock.Substring (0, codeBlock.Length - 2);
					return codeBlock;
				}
			}
			throw new FormatException ("Could not strip block code from: " + line);
		}

		public void LoadInformationFromExistingFile (string fileName)
		{
			LoadModelInformation (fileName);
			LoadCodeBlocks (fileName);
		}

		protected bool IsGuid (string guid)
		{
			string sampleGuid = "\"3d71b08c-7073-4489-8937-58c94a6f80e0\"";
			if (guid.Length == sampleGuid.Length)
			{
				if (guid.StartsWith ("\"") && guid.EndsWith ("\""))
				{
					if (guid.IndexOf ("-") == sampleGuid.IndexOf ("-"))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected void LoadModelInformation (string fileName)
		{
			string hsmGuid = null;
			using (System.IO.StreamReader sr = new System.IO.StreamReader (fileName))
			{
				while (sr.Peek () != -1)
				{
					string rawLine = sr.ReadLine ();
					string line = rawLine.Trim ();
					if (line.StartsWith ("[ModelInformation"))
					{
						string[] strList = line.Split (',');
						if (strList.Length < 3)
						{
							// expect line to have [ModelInformation (@filename, guid, version)]
							throw new ArgumentException ("Expect line to have: [ModelInformation (@filename, guid, version)]\n" + line);
						}
						string guid = strList [1];
						guid = guid.Trim ();
						if (!IsGuid (guid))
						{
							throw new ArgumentException ("Argument is not a guid: " + guid);
						}
						hsmGuid = guid;
					}
				}
			}
			if (hsmGuid == null)
			{
				throw new ArgumentException ("File does not contain a valid ModelInformation attribute");
			} 
			else 
			{
				string modelGuid = "\"" + _Header.ModelGuid + "\"";
				if (hsmGuid != modelGuid)
				{
					throw new ArgumentException ("File " + fileName + " contains a Model Guid " + hsmGuid + " that is different to the current model: " + modelGuid);
				}
			}
		}

		protected void LoadCodeBlocks (string fileName)
		{
			Hashtable codeBlocks = new Hashtable ();
			using (System.IO.StreamReader sr = new System.IO.StreamReader (fileName))
			{
				while (sr.Peek () != -1)
				{
					string rawLine = sr.ReadLine ();
					string line = rawLine.Trim (); 
					if (line.StartsWith ("//Begin[["))
					{
						string codeBlockName = StripCodeBlockName (line, "//Begin[[");
						string endCodeBlock = string.Format ("//End[[{0}]]", codeBlockName);
						StringBuilder block = new StringBuilder ();
						while (sr.Peek () != -1)
						{
							rawLine = sr.ReadLine ();
							line = rawLine.Trim ();
							if (line.StartsWith ("//End[["))
							{
								if (line != endCodeBlock)
								{
									string actualEndCodeBlockName = StripCodeBlockName (line, "//End[[");
									string msg = string.Format ("end block [{0}] does not match begin block [{1}]", actualEndCodeBlockName, codeBlockName);
									throw new FormatException (msg);
								}
								codeBlocks.Add (codeBlockName, new CodeBlock (block.ToString ()));
								break;
							} 
							else 
							{
								block.Append (rawLine + "\n");
							}
						}
					}								  
				}
			}
			_CodeBlocks = codeBlocks;
		}
	}
}
