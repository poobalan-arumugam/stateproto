using System;
using System.Collections;
using System.Text;
using MurphyPA.H2D.Interfaces;
using System.IO;
using System.Xml;
using System.Reflection;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ConvertToXml.
	/// </summary>
	public class ConvertToXml : ConvertToCodeBase
	{
		public ConvertToXml(DiagramModel model)
			: base (model.GetGlyphsList ())
		{
		}

		public class PropertyVisitor : IGlyphVisitor
		{
			XmlTextWriter _Writer;
			public PropertyVisitor (XmlTextWriter writer)
			{
				_Writer = writer;
			}

			protected bool AcceptablePropertyType (PropertyInfo pinfo)
			{
				bool acceptable = pinfo.PropertyType.IsPrimitive;
				acceptable = acceptable || pinfo.PropertyType == typeof (string);
				return acceptable;
			}

			protected void VisitInner (IGlyph glyph)
			{
				foreach (PropertyInfo pinfo in glyph.GetType ().GetProperties ())
				{
					if (AcceptablePropertyType (pinfo))
					{
						try 
						{
							object value = pinfo.GetValue (glyph, null);
							string sv = string.Format ("{0}", value);
							_Writer.WriteElementString (pinfo.Name, sv);
						} 
						catch (Exception ex)
						{
							_Writer.WriteElementString (pinfo.Name, "EXCEPTION: " + ex.ToString ());
						}
					}
				}
			}

			protected void WriteDefaults (IGlyph glyph)
			{
				_Writer.WriteAttributeString ("Name", glyph.Name);
				_Writer.WriteAttributeString ("Id", glyph.Id);
				_Writer.WriteAttributeString ("DoNotInstrument", glyph.DoNotInstrument.ToString ());

				_Writer.WriteElementString ("Note", glyph.Note);

				if (glyph.Owner != null)
				{
					_Writer.WriteElementString ("OwnerId", glyph.Owner.Id);
				}
				if (glyph.Parent != null)
				{
					_Writer.WriteElementString ("ParentId", glyph.Parent.Id);
				}
			}

			#region IGlyphVisitor Members

			public void Visit(IPortLinkGlyph portLink)
			{
				WriteDefaults (portLink);
				_Writer.WriteElementString ("FromPortName", portLink.FromPortName);
				_Writer.WriteElementString ("SendIndex", portLink.SendIndex);
				_Writer.WriteElementString ("ToPortName", portLink.ToPortName);

				foreach (IPortLinkContactPointGlyph contactPoint in portLink.ContactPoints)
				{
					if (contactPoint.Parent != null)
					{
						IComponentGlyph comp = contactPoint.Parent as IComponentGlyph;
						System.Diagnostics.Debug.Assert (comp != null);
						_Writer.WriteStartElement ("Component");
						try
						{
							_Writer.WriteElementString ("Id", comp.Id);
							_Writer.WriteElementString ("Name", comp.Name);
						} 
						finally
						{
							_Writer.WriteEndElement ();
						}
					}
				}
			}

			public void Visit(IPortLinkContactPointGlyph portLinkContactPoint)
			{
			}

			public void Visit(ITransitionContactPointGlyph transitionContactPoint)
			{
			}

			public void Visit(ITransitionGlyph transition)
			{
				WriteDefaults (transition);
				_Writer.WriteElementString ("EventSignal", transition.EventSignal);
				_Writer.WriteElementString ("EventSource", transition.EventSource);
				_Writer.WriteElementString ("GuardCondition", transition.GuardCondition);
				_Writer.WriteElementString ("Action", transition.Action);
				_Writer.WriteElementString ("EvaluationOrderPriority", transition.EvaluationOrderPriority.ToString ());
				_Writer.WriteElementString ("EventType", transition.EventType);
				_Writer.WriteElementString ("IsInnerTransition", transition.IsInnerTransition.ToString ());
				_Writer.WriteElementString ("TimeOutExpression", transition.TimeOutExpression);
				_Writer.WriteElementString ("TransitionType", transition.TransitionType.ToString ());

				foreach (ITransitionContactPointGlyph contactPoint in transition.ContactPoints)
				{
					if (contactPoint.Parent != null)
					{
						IStateGlyph state = contactPoint.Parent as IStateGlyph;
						System.Diagnostics.Debug.Assert (state != null);
						_Writer.WriteStartElement ("State");
						try
						{
							_Writer.WriteElementString ("Id", state.Id);
							_Writer.WriteElementString ("Name", state.Name);
						} 
						finally
						{
							_Writer.WriteEndElement ();
						}
					}
				}
			}

			public void Visit (IComponentGlyph component)
			{
				WriteDefaults (component);
				_Writer.WriteElementString ("TypeName", component.TypeName);
                _Writer.WriteElementString ("IsMultiInstance", component.IsMultiInstance.ToString ());
			}

			public void Visit (IStateTransitionPortGlyph port)
			{
				WriteDefaults (port);
				_Writer.WriteElementString ("IsMultiPort", port.IsMultiPort.ToString ());
			}

			public void Visit(IStateGlyph state)
			{
				WriteDefaults (state);
				_Writer.WriteElementString ("IsStartState", state.IsStartState.ToString ());
				_Writer.WriteElementString ("EntryAction", state.EntryAction);
				_Writer.WriteElementString ("ExitAction", state.ExitAction);
				_Writer.WriteStartElement ("StateCommands");
				try 
				{
					foreach (string cmd in state.StateCommands)
					{
						if (cmd != null && cmd.Trim ().Length > 0)
						{
							_Writer.WriteElementString ("Command", cmd);
						}
					}
				} 
				finally 
				{
					_Writer.WriteEndElement ();
				}
			}

			public void Visit(IGroupGlyph group)
			{
			}

			public void Visit(ICompositeGlyph composite)
			{
			}

			public void Visit(IGlyph glyph)
			{
			}
			#endregion
		}


		protected void WriteGlyphProperties (XmlTextWriter writer, IGlyph glyph)
		{
			IGlyphVisitor visitor = new PropertyVisitor (writer);
			glyph.Accept (visitor);
		}

		protected void WriteGlyphXml (XmlTextWriter writer, IGlyph glyph)
		{
			writer.WriteStartElement (glyph.GetType ().Name);
			try 
			{
				WriteGlyphProperties (writer, glyph);

				foreach (IGlyph child in glyph.Children)
				{
					// only internal glyphs are owned i.e. contact points, etc. - so skip them.
					if (child.Owner == null)
					{
						
						WriteGlyphXml (writer, child);
					}
				}

			}
			finally 
			{
				writer.WriteEndElement ();
			}
		}

		public string Convert ()
		{
			PrepareGlyphs ();

			MemoryStream ms = new MemoryStream ();
			XmlTextWriter writer = new XmlTextWriter (ms, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;

			writer.WriteStartDocument (true);
			writer.WriteStartElement ("Glyphs");
			try
			{
				foreach (IGlyph glyph in _Glyphs)
				{
					if (glyph.Parent == null && glyph.Owner == null)
					{
						WriteGlyphXml (writer, glyph);
					}
				}
			} 
			finally
			{
				writer.WriteEndElement ();
				writer.WriteEndDocument ();
			}

			writer.Flush ();

			ms.Seek (0, SeekOrigin.Begin);

			StreamReader sr = new StreamReader (ms);
			string result = sr.ReadToEnd ();
			sr.Close ();
			writer.Close ();

			return result;
		}

	}
}
