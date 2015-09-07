using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for StateTransitionPortGlyph.
	/// </summary>
	public class StateTransitionPortGlyph : SquareGlyph, IStateTransitionPortGlyph
	{
		public StateTransitionPortGlyph ()
		: base (new Rectangle (10, 10, 50, 50)) 
		{}

		public StateTransitionPortGlyph (string id, Rectangle bound)
			: base (id, bound)
		{
		}

		public override void Accept(IGlyphVisitor visitor)
		{
			visitor.Visit (this);
		}

		public override void Draw(IGraphicsContext GC)
		{
			using (GC.PushGraphicsState ())
			{
				string name = Name;
				if (IsNotEmptyString (name))
				{
					GC.Color = Color.Orange;
				} 
				else 
				{
					name = "NoName";
					GC.Color = Color.Red;
				}
				base.Draw (GC);
				using (Brush brush = new SolidBrush (GC.Color))
				{
					Rectangle bounds = Bounds;
					Point centre = new Point (bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
					GC.DrawString (name, brush, 10, centre, true);
				}
			}
		}

		bool _IsMultiPort;
		public bool IsMultiPort { get { return _IsMultiPort; } set { _IsMultiPort = value; } }
	}
}
