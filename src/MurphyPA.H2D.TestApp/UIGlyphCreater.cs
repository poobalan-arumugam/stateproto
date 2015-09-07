using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for UIGlyphCreater.
	/// </summary>
	public class UIGlyphCreater : UIGlyphKeyboardInputInteractor, IUIInteractionHandler
	{
		string _CreateMethod;
		UISelectorBand _SelectorBand;

		public UIGlyphCreater(IUIInterationContext context, string modelElementMethod)
			: base (context) 
		{
			_SelectorBand = new UISelectorBand (context, true);
			_CreateMethod = modelElementMethod;
		}

		#region IUIInteractionHandler Members

		IGlyphFactory _GlyphFactory = new Implementation.DefaultGlyphFactory ();

		public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_Model.Header.ReadOnly)
			{
				return;
			}

			_SelectorBand.MouseDown (sender, e);
		}

		public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_Model.Header.ReadOnly)
			{
				return;
			}

			_SelectorBand.MouseMove (sender, e);
			_Context.RefreshView ();
		}

		protected bool _IsDirectionalGlyph;

		protected void CalculateIsDirectional (System.Reflection.MethodInfo mInfo)
		{
			object[] directionalAttributes = mInfo.GetCustomAttributes (typeof (DirectionalGlyphAttribute), true);
			_IsDirectionalGlyph = directionalAttributes != null && directionalAttributes.Length > 0;
		}

		protected IGlyph InternalMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			IGlyph createdGlyph = null;

			if (_CreateMethod != "")
			{
				Type type = typeof (IGlyphFactory);
				object glyphObj = null;
                if (_SelectorBand.Banding 
                    && 
                    (
                    _SelectorBand.SelectionBand.Width > 0
                    ||
                    (_SelectorBand.SelectionBand.Width == 0 && _SelectorBand.SelectionBand.Height > 0)
                    )
                    )
                {
                    Type[] types = new Type[] {typeof (string), typeof (Rectangle)};
                    System.Reflection.MethodInfo mInfo = type.GetMethod (_CreateMethod, types);
                    string id = Guid.NewGuid ().ToString ();
                    Rectangle bounds = _SelectorBand.SelectionBand;

                    CalculateIsDirectional (mInfo);
                    if (_IsDirectionalGlyph)
                    {
                        bounds = _SelectorBand.DirectedBand;
                    }
                    object[] args = new object[] {id, bounds};
                    glyphObj = mInfo.Invoke (_GlyphFactory, args);
                } 
                else 
                {
                    Type[] types = new Type[] {typeof (string), typeof (Point)};
                    System.Reflection.MethodInfo mInfo = type.GetMethod (_CreateMethod, types);
                    string id = Guid.NewGuid ().ToString ();
                    Point point = new Point (e.X, e.Y);
                    object[] args = new object[] {id, point};
                    glyphObj = mInfo.Invoke (_GlyphFactory, args);
                }
				IGlyph glyph = glyphObj as IGlyph;
				if (glyph == null)
				{
					throw new NullReferenceException (glyphObj + " is not a glyph or is null");
				}
				_Model.AddGlyph (glyph);
				createdGlyph = glyph;
			}

			return createdGlyph;
		}

		IUIInteractionHandler _Mover;

		protected System.Windows.Forms.MouseEventArgs CreateMouseEventArgs (System.Windows.Forms.MouseEventArgs e, Point point)
		{
			System.Windows.Forms.MouseEventArgs es = new System.Windows.Forms.MouseEventArgs (e.Button, e.Clicks, point.X, point.Y, e.Delta);
			return es;
		}

		protected void DoGlyphCreated (IGlyph createdGlyph)
		{
			GlyphCreatedHandler handler = GlyphCreated;
			if (handler != null)
			{
				handler (createdGlyph);
			}
		}

		public delegate void GlyphCreatedHandler (IGlyph glyph);
		public event GlyphCreatedHandler GlyphCreated;

		public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_Model.Header.ReadOnly)
			{
				return;
			}

			IGlyph createdGlyph = InternalMouseUp (sender, e);

			// set this for keys interactor parent class.
			_LastSelectedGlyph = createdGlyph;

			if (_Mover == null)
			{
				_Mover = new UIGlyphMoveAndReparent (_Context);
			}

			if (createdGlyph != null)
			{
				DoGlyphCreated (createdGlyph);
			}

			if (_IsDirectionalGlyph)
			{
				System.Windows.Forms.MouseEventArgs estart = CreateMouseEventArgs (e, _SelectorBand.StartPoint);
				_Mover.MouseDown (sender, estart);
				_Mover.MouseUp (sender, estart);

				System.Windows.Forms.MouseEventArgs eend = CreateMouseEventArgs (e, _SelectorBand.EndPoint);
				_Mover.MouseDown (sender, eend);
				_Mover.MouseUp (sender, eend);
			} 
			else 
			{
				System.Windows.Forms.MouseEventArgs estart = CreateMouseEventArgs (e, _SelectorBand.SelectionBand.Location);
				_Mover.MouseDown (sender, estart);
				_Mover.MouseUp (sender, estart);
			}

			_Model.DeSelectAllGlyphs ();
			_Context.SelectGlyph (createdGlyph);
			_Context.RefreshView ();

			_SelectorBand.MouseUp (sender, e);
		}

		public override void Draw(IGraphicsContext gc)
		{
			_SelectorBand.Draw (gc);
		}


		#endregion
	}
}
