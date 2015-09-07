using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for CircleGlyph.
	/// </summary>
	public class CircleGlyph : GlyphBase, IGlyph
	{
		protected Point _Centre;
		protected int _Radius;
	    protected int _BoundsExpander;
	    
		public Point Centre { get { return _Centre; } }

		public CircleGlyph(Point centre, int radius, int boundsExpander)
		{
			_Centre = centre;
			_Radius = radius;
		    _BoundsExpander = boundsExpander;
		}

        public CircleGlyph(Point centre, int radius)
	    : this(centre, radius, 5)
        {
        }

	    #region IGlyph Members

		public override void MoveTo(System.Drawing.Point point)
		{
			_Centre = point;
		}

		public override void Offset(System.Drawing.Point offset)
		{
			OffsetEventArgs offsetArgs = new OffsetEventArgs (offset);

			OnBeforeOffsetChanged (offsetArgs);
			_Centre.Offset (offsetArgs.Offset.X, offsetArgs.Offset.Y);
			DoOffsetChanged (offsetArgs);
		}

		public override void Draw(MurphyPA.H2D.Interfaces.IGraphicsContext GC)
		{
			GC.DrawCircle (_Centre, _Radius, true);
		}

		protected override Rectangle GetBounds ()
		{
			Rectangle rect = new Rectangle (_Centre.X - _Radius, _Centre.Y - _Radius, _Radius + _Radius, _Radius + _Radius);
		    rect.Inflate(_BoundsExpander, _BoundsExpander);
			return rect;
		}

		#endregion
	}
}
