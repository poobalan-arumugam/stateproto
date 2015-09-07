using System;
using System.IO;
using System.Drawing;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for LoadGlyphDataFile.
	/// </summary>
	public class LoadGlyphDataFile
	{
		IGlyphFactory _Factory = new Implementation.DefaultGlyphFactory ();

		StateMachineHeader _Header;
		public StateMachineHeader Header { get { return _Header; } }

		ArrayList _Glyphs;
		public ArrayList Glyphs { get { return _Glyphs; } }

		Hashtable _ParentChildRel;
		Hashtable _EntityLinkRel; // entity-link relation like state-transition and component-portlink
		Hashtable _GlyphLookup;

		Queue _Lines = new Queue ();

		protected string NextLine (TextReader sr)
		{
			if (_Lines.Count > 0)
			{
				string line = (string) _Lines.Dequeue ();
				return line;
			}
			return sr.ReadLine ();
		}

		protected void PushBackLine (string line)
		{
			_Lines.Enqueue (line);
		}

		protected void BlankLine (TextReader sr)
		{
			string line = NextLine (sr);
			line = line.Trim ();
			if (line != "")
			{
				throw new FormatException ("Expected blank line");
			}
		}

		protected string LoadText (TextReader sr, string name)
		{
			return LoadText (sr, name, null);
		}

		protected string LoadText (TextReader sr, string name, string defaultValue)
		{
			string line = NextLine (sr);
			if (line != "BEGIN " + name)
			{
				PushBackLine (line);
				if (defaultValue != null)
				{
					return defaultValue;
				} 
				else 
				{
					throw new FormatException ("Expected line: BEGIN " + name);
				}
			}
			System.Diagnostics.Debug.Assert (line == "BEGIN " + name);
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			while (true)
			{
				line = NextLine (sr);
				if (line == "END " + name)
				{
					break;
				}
				builder.AppendFormat ("{0}\n", line);
			}
			return builder.ToString ().Trim ();
		}

		public class StringPair 
		{
			public StringPair (string one, string two)
			{
				One = one;
				Two = two;
			}

			public string One;
			public string Two;
		}

		public void Load (string fileName)
		{
			LoadFile (fileName);
		}

		void Init ()
		{
			_ParentChildRel = new Hashtable ();
			_EntityLinkRel = new Hashtable ();
			_GlyphLookup = new Hashtable ();
			_Glyphs = new ArrayList ();
		}

		void LoadHeader (TextReader sr, string fileName)
		{
			_Header = new StateMachineHeader ();
			_Header.Name = LoadText (sr, "StateMachine");
			string implVer = LoadText (sr, "ImplementationVersion");
			System.Diagnostics.Debug.Assert (_Header.ImplementationVersion.CompareTo (implVer) >= 0);
			string smVer = LoadText (sr, "StateMachineVersion");
			_Header.StateMachineVersion = int.Parse (smVer);
			_Header.BaseStateMachine = LoadText (sr, "BaseStateMachine");
			_Header.NameSpace = LoadText (sr, "NameSpace");
			_Header.UsingNameSpaces = LoadText (sr, "UsingNameSpaces");
			_Header.Comment = LoadText (sr, "Comment");
			_Header.Fields = LoadText (sr, "Fields");
			string readOnly = LoadText (sr, "ReadOnly", false.ToString ());
			_Header.ReadOnly = bool.Parse (readOnly);
			_Header.ModelFileName = LoadText (sr, "ModelFileName", fileName);
			_Header.ModelGuid = LoadText (sr, "ModelGuid", Guid.NewGuid ().ToString ());
			string hasSubMachines = LoadText (sr, "HasSubMachines", false.ToString ());
			_Header.HasSubMachines = bool.Parse (hasSubMachines);
			_Header.Assembly = LoadText (sr, "Assembly", "");
		}

		private void LoadFile (string fileName)
		{
			Init ();

			using (TextReader sr = new StreamReader (fileName))
			{
				LoadHeader (sr, fileName);

				int count = int.Parse (NextLine (sr));

				for (int index = 0; index < count; index++)
				{
					string type = NextLine (sr);
					if (type == "STATE:")
					{
						LoadStateGlyph (sr);
					} 
					else if (type == "TRANSITION:")
					{
						LoadTransitionGlyph (sr);
					}
					else if (type == "STATETRANSITIONPORT:")
					{
						LoadStateTransitionPortGlyph (sr);
					}
					else if (type == "COMPONENT:")
					{
						LoadComponentGlyph (sr);
					}
					else if (type == "PORTLINK:")
					{
						LoadPortLinkGlyph (sr);
					}
				}
			}

			BuildGlyphRelationships ();
		}

		void BuildStateParentRelationships ()
		{
			foreach (DictionaryEntry de in _ParentChildRel)
			{
				IGlyph glyph = _GlyphLookup [de.Key] as IGlyph;
				IGlyph parent = _GlyphLookup [de.Value] as IGlyph;
				glyph.Parent = parent;
			}
		}

		void BuildStateTransitionParentRelationships ()
		{
			foreach (DictionaryEntry de in _EntityLinkRel)
			{
				IGlyph glyph = _GlyphLookup [de.Key] as IGlyph;
				StringPair pair = de.Value as StringPair;
				IGlyph parentOne = _GlyphLookup [pair.One] as IGlyph;
				IGlyph parentTwo = _GlyphLookup [pair.Two] as IGlyph;
				IGroupGlyph groupGlyph = glyph as IGroupGlyph;
				int index = 0;
				foreach (IGlyph contact in groupGlyph.ContactPoints)
				{
					switch (index)
					{
						case 0: 
						{
							contact.Parent = parentOne;
						} break;
						case 1: 
						{
							contact.Parent = parentTwo;
						} break;
					}
					index++;
				}
			}
		}

		void BuildGlyphRelationships ()
		{
			BuildStateParentRelationships ();
			BuildStateTransitionParentRelationships ();
		}

		void LoadBounds (TextReader sr, out string id, out int X, out int Y, out int width, out int height)
		{
			id = NextLine (sr);
			X = int.Parse (NextLine (sr));
			Y = int.Parse (NextLine (sr));
			width = int.Parse (NextLine (sr));
			height = int.Parse (NextLine (sr));
		}

		void LoadGlyphCommon (TextReader sr, IGlyph glyph)
		{
			glyph.Note = LoadText (sr, "NOTE:", "");

			string doNotInstrument = LoadText (sr, "DEBUG_DONOTINSTRUMENT", false.ToString ());
			glyph.DoNotInstrument = bool.Parse (doNotInstrument);
		}

		void LoadStateGlyph (TextReader sr)
		{
			string id;
			int X, Y, width, height;
			LoadBounds (sr, out id, out X, out Y, out width, out height);
			string parentId = NextLine (sr);

			if (parentId != "NOPARENT")
			{
				_ParentChildRel [id] = parentId;
			}
			IStateGlyph glyph = _Factory.CreateState (id, new Rectangle (X, Y, width, height));

			LoadGlyphCommon (sr, glyph);

			IStateGlyph state = glyph;
			state.Name = LoadText (sr, "NAME");
			string isStart = LoadText (sr, "ISSTARTSTATE");
			state.IsStartState = bool.Parse (isStart);
            string isFinal = LoadText (sr, "ISFINALSTATE", false.ToString ());
            state.IsFinalState = bool.Parse (isFinal);
            state.EntryAction = LoadText (sr, "ENTRY");
			state.ExitAction = LoadText (sr, "EXIT");
			state.DoAction = LoadText (sr, "DO");
			string isOverriding = LoadText (sr, "ISOVERRIDING");
			state.IsOverriding = bool.Parse (isOverriding);

			string cmds = LoadText (sr, "STATECOMMANDS", "");
			state.StateCommands.Clear ();
			cmds = cmds.Trim ();
			if (cmds != null && cmds.Trim () != "")
			{
				string[] commandsArray = cmds.Split (';');
				state.StateCommands.AddRange (commandsArray);
			}
			// kludge - when stringcollection displayed in propertygrid - cannot create new string instances - so adding 10 by default (beyond what is already there).
			state.StateCommands.AddRange (new string[] {"", "", "", "", "", "", "", "", "", ""});

			BlankLine (sr); // blank line
			_GlyphLookup.Add (id, glyph);
			_Glyphs.Add (glyph);
		}

		void LoadTransitionGlyph (TextReader sr)
		{
			string id;
			int X, Y, width, height;
			LoadBounds (sr, out id, out X, out Y, out width, out height);
			string parentIdFrom = NextLine (sr);
			string parentIdTo = NextLine (sr);

			_EntityLinkRel [id] = new StringPair (parentIdFrom, parentIdTo);
			ITransitionGlyph glyph = _Factory.CreateTransition (id, new Rectangle (X, Y, width, height));

			LoadGlyphCommon (sr, glyph);

			ITransitionGlyph trans = glyph;
			trans.Name = LoadText (sr, "NAME");
			trans.EventSignal = LoadText (sr, "EVENT");
			trans.GuardCondition = LoadText (sr, "GUARD");
			trans.Action = LoadText (sr, "ACTION");
			string transType = LoadText (sr, "TRANSITIONTYPE");
			trans.TransitionType = (TransitionType) Enum.Parse (typeof (TransitionType), transType, true);
			trans.EventSource = LoadText (sr, "EVENTSOURCE");
			trans.EventType = LoadText (sr, "EVENTTYPE", "");
			string evalOrder = LoadText (sr, "EVALUATIONORDERPRIORITY");
			trans.EvaluationOrderPriority = int.Parse (evalOrder);
			trans.IsInnerTransition = bool.Parse (NextLine (sr));

			string timeoutExpression = LoadText (sr, "AFTERTIMEOUT", ""); // this needs to be here because AFTERTIMEOUT being replaced by TIMEOUTEXPRESSION
			timeoutExpression = ClearZeroTimeOut (timeoutExpression);
			timeoutExpression = LoadText (sr, "TIMEOUTEXPRESSION", timeoutExpression);
			timeoutExpression = ClearZeroTimeOut (timeoutExpression);
			trans.TimeOutExpression = timeoutExpression;

			BlankLine (sr); // blank line
			_GlyphLookup.Add (id, glyph);
			_Glyphs.Add (glyph);
		}

		string ClearZeroTimeOut (string timeoutExpression)
		{
			timeoutExpression = timeoutExpression.Trim ();
			if (timeoutExpression == "0")
			{
				return "";
			}
			return timeoutExpression;
		}

		void LoadStateTransitionPortGlyph (TextReader sr)
		{
			string id;
			int X, Y, width, height;
			LoadBounds (sr, out id, out X, out Y, out width, out height);
			IStateTransitionPortGlyph glyph = _Factory.CreateStateTransitionPort (id, new Rectangle (X, Y, width, height));

			LoadGlyphCommon (sr, glyph);

			IStateTransitionPortGlyph port = glyph;
			port.Name = LoadText (sr, "NAME");
			string isMultiPort = LoadText (sr, "ISMULTIPORT", false.ToString ());
			port.IsMultiPort = bool.Parse (isMultiPort);

			BlankLine (sr); // blank line
			_GlyphLookup.Add (id, glyph);
			_Glyphs.Add (glyph);
		}

		void LoadComponentGlyph (TextReader sr)
		{
			string id;
			int X, Y, width, height;
			LoadBounds (sr, out id, out X, out Y, out width, out height);

			string parentId = NextLine (sr);

			if (parentId != "NOPARENT")
			{
				_ParentChildRel [id] = parentId;
			}
			IComponentGlyph glyph = _Factory.CreateComponent (id, new Rectangle (X, Y, width, height));

			LoadGlyphCommon (sr, glyph);

			IComponentGlyph comp = glyph;
			comp.Name = LoadText (sr, "NAME");
			comp.TypeName = LoadText (sr, "TYPENAME", "");
            string isMultiInstance = LoadText (sr, "ISMULTIINSTANCE", false.ToString ());
            comp.IsMultiInstance = bool.Parse (isMultiInstance);

			BlankLine (sr); // blank line
			_GlyphLookup.Add (id, glyph);
			_Glyphs.Add (glyph);
		}

		void LoadPortLinkGlyph (TextReader sr)
		{
			string id;
			int X, Y, width, height;
			LoadBounds (sr, out id, out X, out Y, out width, out height);
			string parentIdFrom = NextLine (sr);
			string parentIdTo = NextLine (sr);

			_EntityLinkRel [id] = new StringPair (parentIdFrom, parentIdTo);
			IPortLinkGlyph glyph = _Factory.CreatePortLink (id, new Rectangle (X, Y, width, height));

			LoadGlyphCommon (sr, glyph);

			IPortLinkGlyph link = glyph;
			link.Name = LoadText (sr, "NAME");
			link.FromPortName = LoadText (sr, "FROMPORTNAME", "");
			link.ToPortName = LoadText (sr, "TOPORTNAME", "");
			link.SendIndex = LoadText (sr, "SENDINDEX", "");

			BlankLine (sr); // blank line
			_GlyphLookup.Add (id, glyph);
			_Glyphs.Add (glyph);
		}

	}
}
