using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for DrawTrianglePointer.
	/// </summary>
	public class DrawTrianglePointer
	{
		void Calc_DX_DY (int radius, double slope, out float dx, out float dy)
		{
			double hypot = Math.Pow (radius, 2.0);
			double dxsquare = hypot / (1.0 + slope * slope);
			dx = (float)Math.Sqrt (dxsquare);
			dy = (float)Math.Sqrt (hypot - dxsquare);
		}

		void AddSloped (Point from, Point to, int x0, int y0, int triangleBaseWidth, int triangleHeight, double lineSlope, GraphicsPath path)
		{
			double inverseSlope = 1.0 /  lineSlope;

			// calc tip x,y
			float tdy, tdx;
			Calc_DX_DY (triangleHeight, lineSlope, out tdx, out tdy);

			float sdx, sdy;
			Calc_DX_DY (triangleBaseWidth / 2, inverseSlope, out sdx, out sdy);
			// calc right side point

			// up or down
			int uD = to.Y - from.Y > 0 ? -1 : 1;
			// left or right
			int lR = to.X - from.X > 0 ? 1 : -1;

			float fx0 = (float) x0;
			float fy0 = (float) y0;

			path.AddLine (fx0, fy0, x0 + lR * sdx, y0 + uD * sdy);
			path.AddLine (x0 + lR * sdx, y0 + uD * sdy, x0 + lR * tdx, y0 - uD * tdy);
			path.AddLine (x0 + lR * tdx, y0 - uD * tdy, x0 - lR * sdx, y0 - uD * sdy);
			path.AddLine (x0 - lR * sdx, y0 - uD * sdy, fx0, fy0);
		}

		void AddVertical (Point from, Point to, int x0, int y0, int triangleBaseWidth, int triangleHeight, GraphicsPath path)
		{
			// up or down
			int uD = to.Y - from.Y > 0 ? 1 : -1;

			int width = triangleBaseWidth / 2;
			int height = triangleHeight;

			path.AddLine (x0, y0, x0 + width, y0);
			path.AddLine (x0 + width, y0, x0, y0 + uD * height);
			path.AddLine (x0, y0 + uD * height, x0 - width, y0);
			path.AddLine (x0 - width, y0, x0, y0);
		}

		void AddHorizontal (Point from, Point to, int x0, int y0, int triangleBaseWidth, int triangleHeight, GraphicsPath path)
		{
			// left or right
			int lR = to.X - from.X > 0 ? 1 : -1;

			int width = triangleBaseWidth / 2;
			int height = triangleHeight;

			path.AddLine (x0, y0, x0, y0 - width);
			path.AddLine (x0, y0 - width, x0 + lR * height, y0);
			path.AddLine (x0 + lR * height, y0, x0, y0 + width);
			path.AddLine (x0, y0 + width, x0, y0);
		}

		public void Draw (MurphyPA.H2D.Interfaces.IGraphicsContext gc, Point from, Point to, int triangleBaseWidth, int triangleHeight)
		{
			GraphicsPath path = new GraphicsPath ();

			int dY = to.Y - from.Y;
			int dX = to.X - from.X;

			int x0 = to.X;
			int y0 = to.Y;

			if (dX != 0)
			{
				double lineSlope = (double)dY / (double)dX;

				if (Math.Abs (lineSlope) > double.Epsilon)
				{
					AddSloped (from, to, x0, y0, triangleBaseWidth, triangleHeight, lineSlope, path);
				} 
				else 
				{
					// horizontal line
					AddHorizontal (from, to, x0, y0, triangleBaseWidth, triangleHeight, path);
				}
			} 
			else 
			{
				// vertical line
				AddVertical (from, to, x0, y0, triangleBaseWidth, triangleHeight, path);
			}

			path.CloseFigure ();

			using (Brush brush = new System.Drawing.SolidBrush (gc.Color))
			{
				using (Pen pen = new Pen (brush, 5))
				{
					gc.DrawPath (pen, path);
				}
			}
		}
	}
}
