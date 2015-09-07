using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for PortLinkGlyph.
	/// </summary>
	public class PortLinkGlyph : GroupGlyphBase, IPortLinkGlyph
	{
		Point _From = new Point (10, 20);
		Point _To = new Point (100, 22);

		public PortLinkGlyph ()
		{
			BuildContactPoints ();
		}

		public PortLinkGlyph (string id, Rectangle bounds) 
			: base (id)
		{
			_From = bounds.Location;
			_To = new Point (bounds.X + bounds.Width, bounds.Y + bounds.Height);
			BuildContactPoints ();
		}

		public override void Accept(IGlyphVisitor visitor)
		{
			visitor.Visit (this);
		}

		protected void BuildContactPoints ()
		{
			int radius = 5;
			PortLinkContactPointGlyph fromPoint = new PortLinkContactPointGlyph (_From, radius - 1, this, TransitionContactEnd.From, null);
			AddContactPoint (fromPoint);
			AddContactPoint (new PortLinkContactPointGlyph (_To, radius, this, TransitionContactEnd.To, fromPoint));
		}

		protected override void contactPoint_OffsetChanged(IGlyph glyph, OffsetEventArgs offsetEventArgs)
		{
			int index = _ContactPoints.IndexOf (glyph);
			switch (index)
			{
				case 0: 
				{
					_From.Offset (offsetEventArgs.Offset.X, offsetEventArgs.Offset.Y);
				} break;
				case 1: 
				{
					_To.Offset (offsetEventArgs.Offset.X, offsetEventArgs.Offset.Y);
				} break;
			}
		}

		#region IGlyph Members

		protected override Rectangle GetBounds ()
		{
			return new Rectangle (_From, new Size (_To.X - _From.X, _To.Y - _From.Y));
		}

		public override void MoveTo(Point point)
		{
			// TODO:  Add TransitionGlyph.MoveTo implementation
		}

		public override void Offset(Point point)
		{
			_From.Offset (point.X, point.Y);
			_To.Offset (point.X, point.Y);
		}

		public override void Draw(IGraphicsContext GC)
		{
			Color oldColor = GC.Color;
			Color primary = Color.Orchid;
			if (!IsNotEmptyString (FromPortName) || !IsNotEmptyString (ToPortName) /* || !IsNotEmptyString (Interface) */)
			{
				primary = Color.Red;
			}

			GC.Color = primary;
			if (Selected)
			{
				GC.Thickness = 5;
			} 
			else 
			{
				GC.Thickness = 2;
			}
			GC.DrawLine (_From, _To);
			foreach (IPortLinkContactPointGlyph contact in ContactPoints)
			{
				switch (contact.WhichEnd)
				{
					case TransitionContactEnd.From: GC.Thickness = 3; break;
					case TransitionContactEnd.To: GC.Thickness = 5; break;
					default: throw new NotSupportedException ("Unknown TransitionContactEnd: " + contact.WhichEnd.ToString ());
				}
				IComponentGlyph component = contact.Parent as IComponentGlyph;
				if (component != null)
				{
					GC.Color = component.ComponentColor;
					contact.Draw (GC);
				}
				else
				{
					GC.Color = primary;
					contact.Draw (GC);
				}
			}
			string s = DisplayText ();
			using (Brush brush = new System.Drawing.SolidBrush (Color.Black))
			{
				GC.DrawString (s, brush, 10, new Point ((_To.X + _From.X) / 2, (_To.Y + _From.Y) / 2), true);
			}
			GC.Color = oldColor;
		}

		bool IsValidText (string text)
		{
			if (text == null || text.Trim () == "")
			{
				return false;
			}
			return true;
		}

		public string DisplayText () 
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			if (IsValidText (Name))
			{
				builder.AppendFormat ("{0}-", Name);
			}
			builder.AppendFormat ("{0}", Interface);
			return builder.ToString ().Trim ();
		}

		#endregion

		string _Interface;
		[Category ("Port")]
		public string Interface { get { return _Interface; } set { _Interface = value; } }

		string _FromPortName;
		[Category ("Port")]
		public string FromPortName { get { return _FromPortName; } set { _FromPortName = value; } }

		string _ToPortName;
		[Category ("Port")]
		public string ToPortName { get { return _ToPortName; } set { _ToPortName = value; } }

		string _SendIndex;
		[Category ("Port")]
		public string SendIndex { get { return _SendIndex; } set { _SendIndex = value; } }
	}
}
