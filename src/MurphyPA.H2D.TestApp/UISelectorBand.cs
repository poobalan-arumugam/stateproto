using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for UISelectorBand.
	/// </summary>
	public class UISelectorBand : UIGlyphInteractionHandlerBase, IUIInteractionHandler
	{
		public UISelectorBand (IUIInterationContext context)
			: base (context) {}
			
		public UISelectorBand (IUIInterationContext context, bool drawDirection)
			: base (context) 
		{
			_DrawDirection = drawDirection;
		}

		#region IUIInteractionHandler Members

		Point _SelectionBandStartPoint;
		Rectangle _SelectionBand = new Rectangle (0, 0, 0, 0);
		Point _EndPoint = new Point (0, 0);
		bool _Banding = false;
		bool _DrawDirection = false;

		public bool Banding { get { return _Banding; } }
		public Rectangle SelectionBand { get { return _SelectionBand; } }
		public Point StartPoint { get { return _SelectionBandStartPoint; } }
		public Point EndPoint { get { return _EndPoint; } }
		public Rectangle DirectedBand 
		{
			get 
			{
				Size size = new Size (EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
				Rectangle band = new Rectangle (StartPoint, size);
				return band;
			}
		}

		public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			_SelectionBandStartPoint = new Point (e.X, e.Y);
			_SelectionBand = new Rectangle (e.X, e.Y, 0, 0);
			_Banding = true;
		}

		public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_Banding)
			{
				Point toPoint = new Point (e.X, e.Y);
				Point fromPoint = _SelectionBandStartPoint;
				int left = Math.Min (fromPoint.X, toPoint.X);
				int top = Math.Min (fromPoint.Y, toPoint.Y);
				int right = Math.Max (fromPoint.X, toPoint.X);
				int bottom = Math.Max (fromPoint.Y, toPoint.Y);

				_SelectionBand = new Rectangle (left, top, right - left, bottom - top);
				_EndPoint = new Point (e.X, e.Y);
			}
		}

		public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			_Banding = false;
			_SelectionBand = new Rectangle (e.X, e.Y, 0, 0);
		}

        public override void Draw (IGraphicsContext gc)
        {
            if (_SelectionBand.Width > 0 || _SelectionBand.Height > 0)
            {
                if (_DrawDirection)
                {
                    gc.Color = Color.LightGray;
                    gc.DrawRectangle (_SelectionBand);
                    gc.DrawLine (StartPoint, EndPoint, Color.LightGray, Color.DarkGray);
                    gc.DrawTrianglePointer (StartPoint, EndPoint, 5, 5);
                }
                else
                {
                    gc.Color = Color.Gray;
                    gc.DrawRectangle (_SelectionBand);
                }
            }
        }

		#endregion
	}
}