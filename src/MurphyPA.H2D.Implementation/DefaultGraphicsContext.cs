using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for DefaultGraphicsContext.
	/// </summary>
	public class DefaultGraphicsContext : Interfaces.IGraphicsContext
	{
		Graphics _Graphics;

		public DefaultGraphicsContext(Graphics graphics)
		{
			_Graphics = graphics;
		}

		
		protected class DefaultGraphicsState 
		{
			int _Thickness;
			public int Thickness 
			{ 
				get { return _Thickness; }
				set { _Thickness = value; }
			}

			Color _Color;
			public Color Color 
			{ 
				get
				{
					return _Color;
				} 
				set
				{
					_Color = value;
				} 
			}

			public DefaultGraphicsState (int thickness, Color color)
			{
				_Thickness = thickness;
				_Color = color;
			}
		}

		Stack _GraphicsStateStack = new Stack ();
		public Interfaces.GraphicsStatePop PushGraphicsState ()
		{
			_GraphicsStateStack.Push (new DefaultGraphicsState (thickness, color));
			return new Interfaces.GraphicsStatePop (this);
		}

		public void PopGraphicsState ()
		{
			DefaultGraphicsState state = _GraphicsStateStack.Pop () as DefaultGraphicsState;
			thickness = state.Thickness;
			color = state.Color;
		}

		int thickness = 5;
		public int Thickness 
		{ 
			get { return thickness; }
			set { thickness = Math.Min (Math.Max (value, 1), 100); }
		}

		Color color = Color.Blue;
		public Color Color 
		{ 
			get
			{
				return color;
			} 
			set
			{
				color = value;
			} 
		}

		public void DrawPath (Pen pen, GraphicsPath path)
		{
			_Graphics.DrawPath (pen, path);
		}

		public void DrawLine (Point from, Point to)
		{
			using (Brush brush = new System.Drawing.SolidBrush (color))
			{
				using (Pen pen = new Pen (brush, thickness))
				{
					_Graphics.DrawLine (pen, from, to);
				}
			}
		}

		public void DrawLine (Point from, Point to, Color fromColor, Color toColor, DashStyle dashStyle)
		{
			if (from == to)
			{
				return; // no line here
			}
			using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush (from, to, fromColor, toColor))
			{
				using (Pen pen = new Pen (brush, thickness))
				{
					pen.DashStyle = dashStyle;
					_Graphics.DrawLine (pen, from, to);
				}
			}
		}

		public void DrawLine (Point from, Point to, Color fromColor, Color toColor)
		{
			DrawLine (from, to, fromColor, toColor, DashStyle.Solid);
		}

		public void DrawRectangle (Rectangle rect)
		{
			using (Brush brush = new System.Drawing.SolidBrush (color))
			{
				using (Pen pen = new Pen (brush, thickness))
				{
					_Graphics.DrawRectangle (pen, rect);
				}
			}
		}

		public void DrawCircle (Point centre, int radius, bool fill)
		{
			using (Brush brush = new System.Drawing.SolidBrush (color))
			{
				using (Pen pen = new Pen (brush, thickness))
				{
				    Rectangle rect = new Rectangle(centre.X - radius, centre.Y - radius, radius + radius, radius + radius);
					_Graphics.DrawEllipse (pen, rect);
                    if(fill)
                    {
                        _Graphics.FillEllipse (brush, rect);
                    }
				}
			}
		}

		public void DrawString (string s, Brush brush, int thickness, Point point, bool centreAroundPoint)
		{
			DrawStringContext context = new DrawStringContext (s, thickness, color, FontStyle.Regular);
			DrawString (context, brush, point, centreAroundPoint);
		}

		public void DrawString (IDrawStringContext context, Brush brush, Point point, bool centreAroundPoint)
		{
			Font font = new Font (FontFamily.GenericSansSerif, context.Thickness, context.FontStyle, GraphicsUnit.Pixel);
			if (centreAroundPoint)
			{
				Size size = _Graphics.MeasureString (context.Text, font).ToSize ();
				point.Offset (-size.Width / 2, size.Height / 2);
			}
			_Graphics.DrawString (context.Text, font, brush, point);
		}

		public void DrawString (IEnumerable list, Point point, bool positiveWidthAdjust, bool positiveHeightAdjust, bool centreAroundPoint)
		{
			if (centreAroundPoint)
			{
				int height = 0;
				int width = 0;

				foreach (IDrawStringContext context in list)
				{
					Font font = new Font (FontFamily.GenericSansSerif, context.Thickness, context.FontStyle, GraphicsUnit.Pixel);
					Size size = _Graphics.MeasureString (context.Text, font).ToSize ();
					width += size.Width;
					height = Math.Max (height, size.Height);
				}

				if (!positiveWidthAdjust)
				{
					width = -width;
				}
				if (!positiveHeightAdjust)
				{
					height = -height;
				}

				point.Offset (width / 2, height / 2);
			}
			foreach (IDrawStringContext context in list)
			{
				using (Brush brush = new System.Drawing.SolidBrush (context.Color))
				{
					Font font = new Font (FontFamily.GenericSansSerif, context.Thickness, context.FontStyle, GraphicsUnit.Pixel);
					_Graphics.DrawString (context.Text, font, brush, point);
					Size size = _Graphics.MeasureString (context.Text, font).ToSize ();
					point.Offset (size.Width, 0);
				}
			}
		}

		protected MurphyPA.H2D.Implementation.DrawTrianglePointer _TrianglePointer = new DrawTrianglePointer ();
		public void DrawTrianglePointer (Point from, Point to, int triangleBaseWidth, int triangleHeight)
		{
			_TrianglePointer.Draw (this, from, to, triangleBaseWidth, triangleHeight);
		}
	}
}
