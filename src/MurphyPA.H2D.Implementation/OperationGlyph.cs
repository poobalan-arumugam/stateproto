using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using MurphyPA.H2D.Interfaces;

#warning much of this is copy and paste from other Component glyph - which in turn was a copy of State glyph - this needs refactoring of common/repeating code.

/// operation contains extra children - a OperationPortGlyph - 
/// to which an OperationPortLinkGlyph can hook.

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for OperationGlyph.
	/// </summary>
	public class OperationGlyph : GroupGlyphBase, IOperationGlyph
	{
		Rectangle _Bounds = new Rectangle (10, 10, 150, 150);

		public OperationGlyph ()
		{
			BuildContactPoints ();
		}


		public OperationGlyph (string id, Rectangle bounds) 
			: base (id)
		{
			_Bounds = bounds;
			BuildContactPoints ();
		}

		protected void BuildContactPoints ()
		{
			int halflen = 5;
			int len = halflen * 2;
			AddContactPoint (new OperationContactPointGlyph (new Rectangle (_Bounds.X - halflen, _Bounds.Y + _Bounds.Height / 2 - halflen, len, len), this, new OffsetChangedHandler (horizontal_ContactPoint)));
			AddContactPoint (new OperationContactPointGlyph (new Rectangle (_Bounds.X + _Bounds.Width - halflen, _Bounds.Y + _Bounds.Height / 2 - halflen, len, len), this, new OffsetChangedHandler (horizontal_ContactPoint)));
			AddContactPoint (new OperationContactPointGlyph (new Rectangle (_Bounds.X + _Bounds.Width / 2 - halflen, _Bounds.Y - halflen, len, len), this, new OffsetChangedHandler (vertical_ContactPoint)));
			AddContactPoint (new OperationContactPointGlyph (new Rectangle (_Bounds.X + _Bounds.Width / 2 - halflen, _Bounds.Y + _Bounds.Height - halflen, len, len), this, new OffsetChangedHandler (vertical_ContactPoint)));

		
			halflen = 10;
			len = halflen * 2;
			for (int index = 0; index < 3; index++)
			{
				IGlyph child = new OperationPortGlyph (new Rectangle (_Bounds.X + len, _Bounds.Y + len + len * index * 2, len, len), this, "Param" + index.ToString ());
				child.Parent = this;
			}
		}


		private void horizontal_ContactPoint (IGlyph glyph, OffsetEventArgs offsetEventArgs)
		{
			if (glyph is IOperationContactPointGlyph)
			{
				int expectedYPos = _Bounds.Y + _Bounds.Height / 2;
				int currentYPos = glyph.Bounds.Y + glyph.Bounds.Height / 2;
				offsetEventArgs.Offset = new Point (offsetEventArgs.Offset.X, expectedYPos - currentYPos);
			}
		}

		private void vertical_ContactPoint (IGlyph glyph, OffsetEventArgs offsetEventArgs)
		{
			if (glyph is IOperationContactPointGlyph)
			{
				int expectedXPos = _Bounds.X + _Bounds.Width / 2;
				int currentXPos = glyph.Bounds.X + glyph.Bounds.Width / 2;
				offsetEventArgs.Offset = new Point (expectedXPos - currentXPos, offsetEventArgs.Offset.Y);
			}
		}

		public int CountParentDepth ()
		{
			int depth = 0;
			IGlyph p = _Parent;
			while (p != null)
			{
				depth++;
				p = p.Parent;
			}
			return depth;
		}

		public Color ComponentColor 
		{ 
			get 
			{
				int depth = CountParentDepth ();
				Color color = _Colors [depth];
				return color;
			}
		}

		Color[] _Colors = new Color[] {Color.Blue, Color.Aquamarine, Color.BurlyWood, Color.BlueViolet, Color.MidnightBlue, Color.Moccasin, Color.DarkTurquoise};

		public override void Draw (IGraphicsContext gc)
		{
			int depth = CountParentDepth ();
			Color color = _Colors [depth];
			Color contactColor = color;
			if (Name == null || Name.Trim () == "")
			{
				color = Color.Red;
			}

			gc.Color = color;
			DrawComponent (gc, _Bounds.Left, _Bounds.Top, _Bounds.Width, _Bounds.Height, 20, 2+depth, color);
			gc.Color = contactColor;
			gc.Thickness = 2 + depth;
			foreach (IGlyph contact in ContactPoints)
			{
				contact.Draw (gc);
			}

			foreach (IGlyph child in Children)
			{
				if (child is IOperationPortGlyph)
				{
					child.Draw (gc);
				}
			}
		}

		protected void DrawComponent (IGraphicsContext g, int x_left, int y_top, int width, int height, int radius, int thickness, Color color)
		{
			GraphicsPath path = new GraphicsPath ();
			path.StartFigure ();
			path.AddRectangle (Bounds);
			if (Selected)
			{
				path.AddLine (x_left + 5 + thickness, y_top + height / 2, x_left + 5 + thickness, y_top + radius);
				path.AddLine (x_left + 5 + thickness, y_top + 5 + thickness, x_left + 5 + thickness + width / 2, y_top + 5 + thickness);
				path.AddLine (x_left + 5 + thickness + width / 2, y_top + 5 + thickness, x_left + 5 + thickness, y_top + 5 + thickness);
			}
			path.CloseFigure ();

			using (Brush brush = new System.Drawing.SolidBrush (color))
			{
				using (Pen pen = new Pen (brush, thickness))
				{
					g.DrawPath (pen, path);
					if (Name != null && Name.Trim () != "")
					{
						g.DrawString (Name, brush, radius, new Point (x_left + radius, y_top + 1), false);
					}
				}
			}
		}

		#region IGlyph Members

		public override void MoveTo (Point point)
		{
			_Bounds.Offset (point.X - _Bounds.X, point.Y - _Bounds.Y);
		}

		public override void Offset (Point point)
		{
			_Bounds.Offset (point);

			foreach (IGlyph child in _Children)
			{
				if (!_ContactPoints.Contains (child))
				{
					ReOffsetChild (child, point);
				}
			}
			
			foreach (IGlyph contact in _ContactPoints)
			{
				ReOffsetContactPoint (contact, point);
			}
		}

		#endregion

		void ReOffsetContactPoint (int index, Point offset)
		{
			IGlyph contact = _ContactPoints [index] as IGlyph;
			ReOffsetContactPoint (contact, offset);
		}

		void ReOffsetChild (IGlyph child, Point point)
		{
			child.Offset (point);
		}

		protected override void contactPoint_OffsetChanged(IGlyph glyph, OffsetEventArgs offsetEventArgs)
		{
			Point offset = offsetEventArgs.Offset;
			int index = _ContactPoints.IndexOf (glyph);
			if (glyph is IOperationPortGlyph)
			{
				_Bounds.Offset (offset);
				foreach (IGlyph contactPoint in ContactPoints)
				{
					if (contactPoint is IOperationContactPointGlyph)
					{
						ReOffsetContactPoint (contactPoint, offset);
					}
				}
			} 
			else 
			{
				switch (index)
				{
					case 0: 
					{
						_Bounds.X += offset.X; 
						_Bounds.Width -= offset.X; 
						_Bounds.Height += offset.Y;
						ReOffsetContactPoint (2, offset); 
						ReOffsetContactPoint (3, offset);
					} break;
					case 1: 
					{
						_Bounds.Width += offset.X; 
						_Bounds.Height += offset.Y; 
						ReOffsetContactPoint (2, offset);
						ReOffsetContactPoint (3, offset);
					} break;
					case 2: 
					{ 
						_Bounds.Y += offset.Y; 
						_Bounds.Height -= offset.Y; 
						_Bounds.Width += offset.X; 
						ReOffsetContactPoint (0, offset);
						ReOffsetContactPoint (1, offset);
					} break;
					case 3: 
					{
						_Bounds.Height += offset.Y; 
						_Bounds.Width += offset.X; 
						ReOffsetContactPoint (0, offset);
						ReOffsetContactPoint (1, offset);
					} break;
				}
			}
		}

		protected override Rectangle GetBounds ()
		{
			return _Bounds;
		}
	}
}
