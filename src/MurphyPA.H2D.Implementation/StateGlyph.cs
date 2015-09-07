using System;
using System.ComponentModel;
using System.Drawing;
using MurphyPA.H2D.Interfaces;
using System.Drawing.Drawing2D;
using System.Collections;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for StateGlyph.
	/// </summary>
	public class StateGlyph : GroupGlyphBase, IStateGlyph
	{
		Rectangle _Bounds = new Rectangle (10, 10, 150, 150);

		public StateGlyph ()
		{
			BuildContactPoints ();
		}

		public StateGlyph (string id, Point point) 
			: base (id)
		{
			_Bounds = new Rectangle (point.X, point.Y, _Bounds.Width, _Bounds.Height);
			BuildContactPoints ();
		}

		public StateGlyph (string id, Rectangle bounds) 
			: base (id)
		{
			_Bounds = bounds;
			BuildContactPoints ();
		}

		public override void Accept (IGlyphVisitor visitor)
		{
			visitor.Visit (this);
		}

		protected void BuildContactPoints ()
		{
			int radius = 3;
			AddContactPoint (new StateContactPointCircleGlyph (new Point (_Bounds.X, _Bounds.Y + _Bounds.Height / 2), radius, this, new OffsetChangedHandler (horizontal_ContactPoint)));
			AddContactPoint (new StateContactPointCircleGlyph (new Point (_Bounds.X + _Bounds.Width, _Bounds.Y + _Bounds.Height / 2), radius, this, new OffsetChangedHandler (horizontal_ContactPoint)));
			AddContactPoint (new StateContactPointCircleGlyph (new Point (_Bounds.X + _Bounds.Width / 2, _Bounds.Y), radius, this, new OffsetChangedHandler (vertical_ContactPoint)));
			AddContactPoint (new StateContactPointCircleGlyph (new Point (_Bounds.X + _Bounds.Width / 2, _Bounds.Y + _Bounds.Height), radius, this, new OffsetChangedHandler (vertical_ContactPoint)));
		}


		private void horizontal_ContactPoint (IGlyph glyph, OffsetEventArgs offsetEventArgs)
		{
			int expectedYPos = _Bounds.Y + _Bounds.Height / 2;
			int currentYPos = glyph.Bounds.Y + glyph.Bounds.Height / 2;
			offsetEventArgs.Offset = new Point (offsetEventArgs.Offset.X, expectedYPos - currentYPos);
		}

		private void vertical_ContactPoint (IGlyph glyph, OffsetEventArgs offsetEventArgs)
		{
			int expectedXPos = _Bounds.X + _Bounds.Width / 2;
			int currentXPos = glyph.Bounds.X + glyph.Bounds.Width / 2;
			offsetEventArgs.Offset = new Point (expectedXPos - currentXPos, offsetEventArgs.Offset.Y);
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

		public Color StateColor 
		{ 
			get 
			{
				int depth = CountParentDepth ();
				Color color = _Colors [depth];
				return color;
			}
		}

		ColorDepth _Colors = new ColorDepth ();

		public override void Draw (IGraphicsContext gc)
		{
			using (gc.PushGraphicsState ())
			{
				int depth = CountParentDepth ();
				Color color = _Colors [depth]; // CountParentDepth is expensive - so don't call StateColor here...
				Color contactColor = color;
				if (Name == null || Name.Trim () == "")
				{
					color = Color.Red;
				}

				gc.Color = color;
				DrawState (gc, _Bounds.Left, _Bounds.Top, _Bounds.Width, _Bounds.Height, 20, 2+depth, color, false);
				gc.Color = contactColor;
				gc.Thickness = 2 + depth;
				if (Selected)
				{
					foreach (IGlyph contact in ContactPoints)
					{
						contact.Draw (gc);
					}
				}
			}
		}

		protected void DrawString (IGraphicsContext g, string s, Brush brush, int thickness, Point point, FontStyle fontStyle)
		{
			IDrawStringContext context = new DrawStringContext (s, thickness, StateColor, fontStyle);
			g.DrawString (context, brush, point, false);
		}			   

		protected void DrawState (IGraphicsContext g, int x_left, int y_top, int width, int height, int radius, int thickness, Color color, bool drawTestRect)
		{
			if (drawTestRect)
			{
				GraphicsPath path = new GraphicsPath ();
				Rectangle rect = new Rectangle (x_left, y_top, width, height);
				path.AddRectangle (rect);
				path.CloseFigure ();

				using (Brush brush = new System.Drawing.SolidBrush (Color.Yellow))
				{
					using (Pen pen = new Pen (brush, thickness))
					{
						g.DrawPath (pen, path);
					}
				}
			}

			if (true)
			{
				GraphicsPath path = new GraphicsPath ();
				path.StartFigure ();
				path.AddArc (x_left, y_top, radius, radius, 180, 90);
				if (IsStartState)
				{
					path.AddLine (x_left + radius, y_top, x_left, y_top + radius);
				}
				path.AddLine (x_left + radius, y_top, x_left + width - radius - radius, y_top);
				path.AddArc (x_left + width - radius, y_top, radius, radius, 270, 90);
				path.AddLine (x_left + width, y_top + radius, x_left + width, y_top + height - radius - radius);
				path.AddArc (x_left + width - radius, y_top + height - radius, radius, radius, 0, 90);
				path.AddLine (x_left + width - radius, y_top + height, x_left + radius, y_top + height);
				path.AddArc (x_left, y_top + height - radius, radius, radius, 90, 90);
				path.AddLine (x_left, y_top + height - radius, x_left, y_top + radius);
				if (Selected)
				{
					path.AddLine (x_left + 5 + thickness, y_top + radius, x_left + 5 + thickness, y_top + height / 2);
					path.AddLine (x_left + 5 + thickness, y_top + height / 2, x_left + 5 + thickness, y_top + radius);
				}
                path.CloseFigure ();
                if (IsFinalState)
                {
                    path.StartFigure ();
                    path.AddLine (x_left +width - radius, y_top + height, x_left + width, y_top + height - radius);
                    path.CloseFigure ();
                }

				using (Brush brush = new System.Drawing.SolidBrush (color))
				{
					using (Pen pen = new Pen (brush, thickness))
					{
						g.DrawPath (pen, path);

						if (IsNotEmptyString (Name))
						{
							DrawString (g, Name, brush, (int) ((radius * 2.0) / 3.0), new Point (x_left + radius, y_top + 1), FontStyle.Bold);
						}
						int ypos = y_top + 1 + radius + radius / 2;
						if (IsNotEmptyString (EntryAction))
						{
							DrawString (g, "e:" + EntryAction, brush, radius / 2, new Point (x_left + radius, ypos), FontStyle.Regular);
							ypos += (radius + radius) / 3;
						}
						if (IsNotEmptyString (ExitAction))
						{
							DrawString (g, "x:" + ExitAction, brush, radius / 2, new Point (x_left + radius, ypos), FontStyle.Regular);
							ypos += (radius + radius) / 3;
						}
						if (IsNotEmptyString (DoAction))
						{
							DrawString (g, "a:" + DoAction, brush, radius / 2, new Point (x_left + radius, ypos), FontStyle.Regular);
						}
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

		protected override bool AcceptParentInFullyQualifiedName (IGlyph glyph)
		{
			System.Diagnostics.Debug.Assert (glyph != null);
			IStateGlyph state = glyph as IStateGlyph;
			System.Diagnostics.Debug.Assert (state != null);
			return !state.IsOverriding;
		}

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

		protected override Rectangle GetBounds ()
		{
			return _Bounds;
		}

		bool _IsStartState;
		[Category ("State")]
		public bool IsStartState { get { return _IsStartState; } set { _IsStartState = value; } }

        bool _IsFinalState;
        [Category ("State")]
        public bool IsFinalState { get { return _IsFinalState; } set { _IsFinalState = value; } }

		bool _IsOverriding;
		[Category ("State")]
		public bool IsOverriding { get { return _IsOverriding; } set { _IsOverriding = value; } }

		string _EntryAction = "";
		[Category ("State")]
		public string EntryAction { get { return _EntryAction; } set { _EntryAction = value; } }

		string _ExitAction = "";
		[Category ("State")]
		public string ExitAction { get { return _ExitAction; } set { _ExitAction = value; } }

		string _DoAction = "";
		[Category ("State")]
		public string DoAction { get { return _DoAction; } set { _DoAction = value; } }

		System.Collections.Specialized.StringCollection _StateCommands = new System.Collections.Specialized.StringCollection ();
		[Category ("State")]
		public System.Collections.Specialized.StringCollection StateCommands 
		{ 
			get
			{
				return _StateCommands;
			}
		}
	}
}
