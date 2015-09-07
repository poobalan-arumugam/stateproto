using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for OperationPortContactPointGlyph.
	/// </summary>
	public class OperationPortGlyph : SquareGlyph, IOperationPortGlyph
	{
		string _PortName;

		public OperationPortGlyph (Rectangle bounds, IGlyph parent, string portName)
			: base (bounds)
		{
			this.Parent = parent;
			_PortName = portName;
		}

		public override void Draw(IGraphicsContext GC)
		{
			base.Draw (GC);
			string s = _PortName;
			using (Brush brush = new System.Drawing.SolidBrush (Color.Black))
			{
				GC.DrawString (s, brush, 10, new Point (Bounds.Right + 10, Bounds.Top), false);
			}
		}

	}
}
