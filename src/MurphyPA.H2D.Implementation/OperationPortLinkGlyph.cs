using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for OperationPortLinkGlyph.
	/// </summary>
	public class OperationPortLinkGlyph : GroupGlyphBase, IOperationPortLinkGlyph
	{
		Point _From = new Point (10, 20);
		Point _To = new Point (100, 22);

		public OperationPortLinkGlyph ()
		{
			BuildContactPoints ();
		}

		public OperationPortLinkGlyph (string id, Rectangle bounds) 
			: base (id)
		{
			_From = bounds.Location;
			_To = new Point (bounds.X + bounds.Width, bounds.Y + bounds.Height);
			BuildContactPoints ();
		}

		protected void BuildContactPoints ()
		{
			int radius = 5;
			AddContactPoint (new OperationPortLinkContactPointGlyph (_From, radius - 1, this, TransitionContactEnd.From, _To));
			AddContactPoint (new OperationPortLinkContactPointGlyph (_To, radius, this, TransitionContactEnd.To, _From));
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
			if (Interface == null || Interface.Trim () == "")
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
			foreach (OperationPortLinkContactPointGlyph contact in ContactPoints)
			{
				switch (contact.WhichEnd)
				{
					case TransitionContactEnd.From: GC.Thickness = 3; break;
					case TransitionContactEnd.To: GC.Thickness = 5; break;
					default: throw new NotSupportedException ("Unknown TransitionContactEnd: " + contact.WhichEnd.ToString ());
				}
				IOperationGlyph operation = contact.Parent as IOperationGlyph;
				if (operation != null)
				{
					GC.Color = primary; // FIXUP: operation.OperationColor;
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
	}
}
