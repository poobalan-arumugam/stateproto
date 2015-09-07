using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for PortLinkContactPointGlyph.
	/// </summary>
	public class PortLinkContactPointGlyph : SquareGlyph, IPortLinkContactPointGlyph
	{
		TransitionContactEnd _WhichEnd;
		PortLinkContactPointGlyph _OtherEnd;
		public PortLinkContactPointGlyph (Point centre, int radius, IGlyph parent, TransitionContactEnd whichEnd, PortLinkContactPointGlyph otherEnd)
			: base (new Rectangle (centre.X - radius, centre.Y - radius, radius + radius, radius + radius))
		{
			this.Parent = parent;
			this._WhichEnd = whichEnd;
			this._OtherEnd = otherEnd;
		}

		Point Centre (Rectangle bounds)
		{
			int x = bounds.Left + ((bounds.Right > bounds.Left) ? 1 : -1) * bounds.Width / 2;
			int y = bounds.Top + ((bounds.Bottom > bounds.Top) ? 1 : -1) * bounds.Height / 2;
			return new Point (x, y);
		}

		DrawTrianglePointer pointer = new DrawTrianglePointer ();
		public override void Draw(MurphyPA.H2D.Interfaces.IGraphicsContext GC)
		{
			if (_WhichEnd == TransitionContactEnd.To)
			{
				int height = this.Bounds.Width;
				pointer.Draw (GC, Centre (_OtherEnd.Bounds),  Centre (this.Bounds), height, height);
			} 
			else
			{ 
				base.Draw (GC);
			}

			IPortLinkGlyph portLink = Owner as IPortLinkGlyph;
			if (portLink != null)
			{
				string portName = "?NoName";
				switch (_WhichEnd)
				{
					case TransitionContactEnd.From:
					{
						portName = portLink.FromPortName;
						if (IsNotEmptyString (portLink.SendIndex))
						{
							portName = portName + "-" + portLink.SendIndex;
						}
					} break;
					case TransitionContactEnd.To:
					{
						portName = portLink.ToPortName;
					} break;
				}

				using (Brush brush = new System.Drawing.SolidBrush (GC.Color))
				{
					Rectangle bounds = Bounds;
					GC.DrawString (portName, brush, 12, new Point (bounds.Right, bounds.Bottom), false);
				}
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
