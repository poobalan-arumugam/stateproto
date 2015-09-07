using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IGraphicsContext.
	/// </summary>
	public interface IGraphicsContext
	{
		Color Color { get; set; }
		int Thickness { get; set; }

		GraphicsStatePop PushGraphicsState ();
		void PopGraphicsState ();

		void DrawLine (Point from, Point to);
		void DrawLine (Point from, Point to, Color fromColor, Color toColor);
		void DrawLine (Point from, Point to, Color fromColor, Color toColor, DashStyle dashStyle);
		void DrawRectangle (Rectangle rect);
		void DrawPath (Pen pen, GraphicsPath path);
		void DrawCircle (Point centre, int radius, bool fill);
		void DrawString (string s, Brush brush, int thickness, Point point, bool centreAroundPoint);
		void DrawString (IDrawStringContext context, Brush brush, Point point, bool centreAroundPoint);
		void DrawString (IEnumerable list, Point point, bool positiveWidthAdjust, bool positiveHeightAdjust, bool centreAroundPoint);
		void DrawTrianglePointer (Point from, Point to, int triangleBaseWidth, int triangleHeight);
	}
}
