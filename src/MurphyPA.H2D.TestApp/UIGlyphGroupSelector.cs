using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for UIGlyphGroupSelector.
	/// </summary>
	public class UIGlyphGroupSelector : UIGlyphInteractionHandlerBase, IUIInteractionHandler
	{
		UISelectorBand _SelectorBand;

		public UIGlyphGroupSelector(IUIInterationContext context)
			: base (context) 
		{
			_SelectorBand = new UISelectorBand (context);
		}

		#region IUIInteractionHandler Members

		public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			_Context.ShowHeader ();
			_SelectorBand.MouseDown (sender, e);
		}

		public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			_SelectorBand.MouseMove (sender, e);

			if (_SelectorBand.Banding)
			{
				Rectangle selectionBand = _SelectorBand.SelectionBand;
				if (selectionBand.Width > 0)
				{
					foreach (IGlyph glyph in _Model.Glyphs)
					{
						glyph.Selected = false;
						Rectangle bounds = glyph.Bounds;
						Point rightBottom = new Point (bounds.Right, bounds.Bottom);
						if (selectionBand.Contains (bounds.Location) 
							&& selectionBand.Contains (rightBottom)
							&& selectionBand.Contains (bounds))
						{
							glyph.Selected = true;
						}
					}
				}
			}

			_Context.RefreshView ();	
		}

		public void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			_SelectorBand.MouseUp (sender, e);
		}

		public override void Draw (IGraphicsContext gc)
		{
			_SelectorBand.Draw (gc);
		}

		#endregion
	}
}
