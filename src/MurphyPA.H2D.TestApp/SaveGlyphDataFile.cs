using System;
using System.IO;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for SaveGlyphDataFile.
	/// </summary>
	public class SaveGlyphDataFile
	{
		DiagramModel _Model;
		StateMachineHeader _Header; 
		ArrayList _Glyphs;
	

		public SaveGlyphDataFile(DiagramModel model)
		{
			_Model = model;
			_Header = model.Header;
			_Glyphs = model.GetGlyphsList ();
		}

		protected void SaveTextIfNotDefault (TextWriter sw, string name, string text, string defaultValue)
		{
			if (text != defaultValue)
			{
				SaveText (sw, name, text);
			}
		}

		protected void SaveText (TextWriter sw, string name, string text)
		{
			sw.WriteLine ("BEGIN " + name);
			if (text != null)
			{
				text = text.Trim ();
				if (text != "")
				{
					sw.WriteLine (text);
				}
			}
			sw.WriteLine ("END " + name);
		}

		public void Save (string fileName)
		{
			SaveFile (fileName);
		}

		void SaveHeader (TextWriter sw)
		{
			SaveText (sw, "StateMachine", _Header.Name);
			SaveText (sw, "ImplementationVersion", _Header.ImplementationVersion);
			SaveText (sw, "StateMachineVersion", _Header.StateMachineVersion.ToString ());
			SaveText (sw, "BaseStateMachine", _Header.BaseStateMachine);
			SaveText (sw, "NameSpace", _Header.NameSpace);
			SaveText (sw, "UsingNameSpaces", _Header.UsingNameSpaces);
			SaveText (sw, "Comment", _Header.Comment);
			SaveText (sw, "Fields", _Header.Fields);
			SaveText (sw, "ReadOnly", _Header.ReadOnly.ToString ());
			SaveText (sw, "ModelFileName", _Header.ModelFileName);
			SaveText (sw, "ModelGuid", _Header.ModelGuid);
			SaveText (sw, "HasSubMachines", _Header.HasSubMachines.ToString ());
			SaveText (sw, "Assembly", _Header.Assembly);
		}

		private void SaveToStream (TextWriter sw)
		{
			SaveHeader (sw);

			sw.WriteLine (_Glyphs.Count);

			foreach (IGlyph glyph in _Glyphs)
			{
				if (glyph is IStateGlyph)
				{
					SaveStateGlyph (sw, glyph as IStateGlyph);
				}
				else if (glyph is ITransitionGlyph)
				{
					SaveTransitionGlyph (sw, glyph as ITransitionGlyph);
				}
				else if (glyph is IStateTransitionPortGlyph)
				{
					SaveStateTransitionPortGlyph (sw, glyph as IStateTransitionPortGlyph);
				}
				else if (glyph is IComponentGlyph)
				{
					SaveComponentGlyph (sw, glyph as IComponentGlyph);
				}
				else if (glyph is IPortLinkGlyph)
				{
					SavePortLinkGlyph (sw, glyph as IPortLinkGlyph);
				}
				sw.WriteLine ();
			}
		}

		protected bool HasFileContentChanged (string fileName)
		{
			if (File.Exists (fileName))
			{
				// has the content changed?
				string newText = "";
				using (TextWriter swstr = new StringWriter ())
				{
					SaveToStream (swstr);
					swstr.Flush ();
					newText = swstr.ToString ();
				}
				using (StreamReader sr = new StreamReader (fileName))
				{
					string currentText = sr.ReadToEnd ();
					if (currentText.Trim () == newText.Trim ())
					{
						return false;
					}
				}
			}
			return true;
		}

		private void SaveFile (string fileName)
		{
			if (HasFileContentChanged (fileName))
			{
				_Header.ModelFileName = System.IO.Path.GetFileName (fileName);
				_Header.StateMachineVersion++;
				using (TextWriter sw = new StreamWriter (fileName))
				{
					SaveToStream (sw);
					_Model.IsDirty = false;
				}
			}
		}

		void SaveBounds (TextWriter sw, string elementType, IGlyph glyph)
		{
			sw.WriteLine (elementType);
			sw.WriteLine (glyph.Id);
			sw.WriteLine (glyph.Bounds.X);
			sw.WriteLine (glyph.Bounds.Y);
			sw.WriteLine (glyph.Bounds.Width);
			sw.WriteLine (glyph.Bounds.Height);
		}

		void SaveGlyphCommon (TextWriter sw, IGlyph glyph)
		{
			SaveText (sw, "NOTE:", glyph.Note);
			SaveTextIfNotDefault (sw, "DEBUG_DONOTINSTRUMENT", glyph.DoNotInstrument.ToString (), false.ToString ());
		}

		void SaveStateGlyph (TextWriter sw, IStateGlyph glyph)
		{
			SaveBounds (sw, "STATE:", glyph);
			if (glyph.Parent != null)
			{
				sw.WriteLine (glyph.Parent.Id);
			} 
			else 
			{
				sw.WriteLine ("NOPARENT");
			}

			SaveGlyphCommon (sw, glyph);

			IStateGlyph state = glyph;
			SaveText (sw, "NAME", state.Name);
			SaveText (sw, "ISSTARTSTATE", state.IsStartState.ToString ());
            SaveText (sw, "ISFINALSTATE", state.IsFinalState.ToString ());
            SaveText (sw, "ENTRY", state.EntryAction);
			SaveText (sw, "EXIT", state.ExitAction);
			SaveText (sw, "DO", state.DoAction);
			SaveText (sw, "ISOVERRIDING", state.IsOverriding.ToString ());

			ArrayList commandList = new ArrayList ();
			foreach (string commandName in state.StateCommands)
			{
				if (commandName != null && commandName.Trim () != "")
				{
					commandList.Add (commandName);
				}
			}
			string[] commandsArray = (string[]) commandList.ToArray (typeof (string));
			string cmds = string.Join (";", commandsArray);
			SaveTextIfNotDefault (sw, "STATECOMMANDS", cmds, "");
		}

		void SaveTransitionGlyph (TextWriter sw, ITransitionGlyph glyph)
		{
			SaveBounds (sw, "TRANSITION:", glyph);
			IGroupGlyph groupGlyph = glyph as IGroupGlyph;
			foreach (IGlyph contact in groupGlyph.ContactPoints)
			{
				if (contact.Parent != null)
				{
					sw.WriteLine (contact.Parent.Id);
				}
				else 
				{
					sw.WriteLine ("NOPARENT");
				}
			}

			SaveGlyphCommon (sw, glyph);

			ITransitionGlyph trans = glyph;
			SaveText (sw, "NAME", trans.Name);
			SaveText (sw, "EVENT", trans.EventSignal);
			SaveText (sw, "GUARD", trans.GuardCondition);
			SaveText (sw, "ACTION", trans.Action);
			SaveText (sw, "TRANSITIONTYPE", trans.TransitionType.ToString ());
			SaveText (sw, "EVENTSOURCE", trans.EventSource);
			SaveTextIfNotDefault (sw, "EVENTTYPE", trans.EventType, "");
			SaveText (sw, "EVALUATIONORDERPRIORITY", trans.EvaluationOrderPriority.ToString ());
			sw.WriteLine (trans.IsInnerTransition);
			SaveText (sw, "TIMEOUTEXPRESSION", trans.TimeOutExpression);
		}

		void SaveStateTransitionPortGlyph (TextWriter sw, IStateTransitionPortGlyph glyph)
		{
			SaveBounds (sw, "STATETRANSITIONPORT:", glyph);

			SaveGlyphCommon (sw, glyph);

			IStateTransitionPortGlyph port = glyph;
			SaveText (sw, "NAME", port.Name);
			SaveTextIfNotDefault (sw, "ISMULTIPORT", port.IsMultiPort.ToString (), false.ToString ());
		}

		void SaveComponentGlyph (TextWriter sw, IComponentGlyph glyph)
		{
			SaveBounds (sw, "COMPONENT:", glyph);
			if (glyph.Parent != null)
			{
				sw.WriteLine (glyph.Parent.Id);
			} 
			else 
			{
				sw.WriteLine ("NOPARENT");
			}

			SaveGlyphCommon (sw, glyph);

			IComponentGlyph comp = glyph;
			SaveText (sw, "NAME", comp.Name);
			SaveText (sw, "TYPENAME", comp.TypeName);
            SaveTextIfNotDefault (sw, "ISMULTIINSTANCE", comp.IsMultiInstance.ToString (), false.ToString ());
		}

		void SavePortLinkGlyph (TextWriter sw, IPortLinkGlyph glyph)
		{
			SaveBounds (sw, "PORTLINK:", glyph);
			IGroupGlyph groupGlyph = glyph as IGroupGlyph;
			foreach (IGlyph contact in groupGlyph.ContactPoints)
			{
				if (contact.Parent != null)
				{
					sw.WriteLine (contact.Parent.Id);
				}
				else 
				{
					sw.WriteLine ("NOPARENT");
				}
			}

			SaveGlyphCommon (sw, glyph);

			IPortLinkGlyph link = glyph;
			SaveText (sw, "NAME", link.Name);
			SaveText (sw, "FROMPORTNAME", link.FromPortName);
			SaveText (sw, "TOPORTNAME", link.ToPortName);
			SaveText (sw, "SENDINDEX", link.SendIndex);
		}

	}
}
