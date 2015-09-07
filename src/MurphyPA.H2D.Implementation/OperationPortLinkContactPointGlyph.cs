using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for OperationPortLinkContactPointGlyph.
	/// </summary>
	public class OperationPortLinkContactPointGlyph : SquareGlyph, IOperationPortLinkContactPointGlyph
	{
		TransitionContactEnd _WhichEnd;
		Point _OtherEnd;
		public OperationPortLinkContactPointGlyph (Point centre, int radius, IGlyph parent, TransitionContactEnd whichEnd, Point otherEnd)
			: base (new Rectangle (centre.X - radius, centre.Y - radius, radius + radius, radius + radius))
		{
			this.Parent = parent;
			this._WhichEnd = whichEnd;
			this._OtherEnd = otherEnd;
		}

		public override void Draw(MurphyPA.H2D.Interfaces.IGraphicsContext GC)
		{
			/*
			if (_WhichEnd == TransitionContactEnd.To)
			{		
			// want to draw an arrow here...		

				GraphicsPath path = new GraphicsPath ();
				path.AddLine ();
				path.AddLine ();
				path.AddLine ();
				path.CloseFigure ();

				using (Brush brush = new System.Drawing.SolidBrush (GC.Color))
				{
					using (Pen pen = new Pen (brush, 5))
					{
						GC.DrawPath (pen, path);
					}
				}
			} 
			else 
				*/
		{
			base.Draw (GC);
		}
		}

		public TransitionContactEnd WhichEnd 
		{ 
			get
			{
				return _WhichEnd; 
			}
		}
	}
}
