using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for CircleGlyph.
	/// </summary>
	public class TransitionContactPointCircleGlyph : CircleGlyph, ITransitionContactPointGlyph
	{
		TransitionContactEnd _WhichEnd;
		TransitionContactPointCircleGlyph _OtherEnd;
		public TransitionContactPointCircleGlyph (Point centre, int radius, IGlyph parent, TransitionContactEnd whichEnd, TransitionContactPointCircleGlyph otherEnd)
			: base (centre, radius)
		{
			this.Parent = parent;
			this._WhichEnd = whichEnd;
			this._OtherEnd = otherEnd;
		}

		DrawTrianglePointer pointer = new DrawTrianglePointer ();
		public override void Draw(MurphyPA.H2D.Interfaces.IGraphicsContext GC)
		{
			if (_WhichEnd == TransitionContactEnd.To)
			{		
			// want to draw an arrow here...		

				pointer.Draw (GC, _OtherEnd.Centre, this.Centre, _Radius, _Radius);
			} 
			else 
			{
				base.Draw (GC);
			}

			if (_WhichEnd == TransitionContactEnd.To)
			{
				ITransitionGlyph trans = Owner as ITransitionGlyph;
				if (trans != null && trans.TransitionType == TransitionType.DeepHistory)
				{
					using (Brush brush = new System.Drawing.SolidBrush (GC.Color))
					{
						GC.DrawString ("H*", brush, 12, _Centre, false);
					}
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
