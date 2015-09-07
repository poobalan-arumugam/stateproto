using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for SquareGlyph.
	/// </summary>
	public class SquareGlyph : GlyphBase, IGlyph
	{
		protected Rectangle _Bounds;

		public SquareGlyph(Rectangle bounds)
		{
			_Bounds = bounds;
		}

		public SquareGlyph(string id, Rectangle bounds)
			: base (id)
		{
			_Bounds = bounds;
		}

		#region IGlyph Members

		public override void MoveTo(System.Drawing.Point point)
		{
			_Bounds.Location = point;
		}

		public override void Offset(System.Drawing.Point offset)
		{
			OffsetEventArgs offsetArgs = new OffsetEventArgs (offset);

			OnBeforeOffsetChanged (offsetArgs);
			_Bounds.Offset (offsetArgs.Offset.X, offsetArgs.Offset.Y);
			DoOffsetChanged (offsetArgs);
		}

		public override void Draw(MurphyPA.H2D.Interfaces.IGraphicsContext GC)
		{
			GC.DrawRectangle (_Bounds);
		}

		protected override Rectangle GetBounds ()
		{
			return _Bounds;
		}

		#endregion
	}
}
