using System;
using System.Drawing;
using System.Windows.Forms;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for UIGlyphMoveAndReparent.
	/// </summary>
	public class UIGlyphMoveAndReparent : UIGlyphKeyboardInputInteractor, IUIInteractionHandler
	{
		IGlyph _CurrentGlyph;
		IGlyph _ActualCurrentGlyph;
		Point _Point;

		public UIGlyphMoveAndReparent(IUIInterationContext context)
			: base (context) {}

		#region IUIInteractionHandler Members

		public void MouseDown(object sender, MouseEventArgs e)
		{
			_CurrentGlyph = null;

			_Model.DeSelectAllGlyphs ();

			_Point = new Point (e.X, e.Y);
			IGlyph actualCurrentGlyph;
			_CurrentGlyph = _Model.FindContactPoint (_Point, out actualCurrentGlyph);
			_ActualCurrentGlyph = actualCurrentGlyph;

			if (_CurrentGlyph == null)
			{
				_CurrentGlyph = _Model.FindGlyph (_Point);
				_ActualCurrentGlyph = _CurrentGlyph;
			}
			if (_CurrentGlyph != null)
			{
				System.Diagnostics.Debug.Assert (_ActualCurrentGlyph != null);
				_Context.SelectGlyph (_ActualCurrentGlyph);
				_LastSelectedGlyph = _ActualCurrentGlyph;
			} 
			else 
			{
				_Context.ShowHeader ();
			}
		}

		public void MouseMove(object sender, MouseEventArgs e)
		{
			if (_Model.Header.ReadOnly)
			{
				return;
			}

			if (_CurrentGlyph != null)
			{
				Point offset = new Point (e.X - _Point.X, e.Y - _Point.Y);
				_CurrentGlyph.Offset (offset);
				_Point = new Point (e.X, e.Y);
				_Context.RefreshView ();
			}
		}

        Rectangle GetReparentBounds (IGlyph currentGlyph)
        {
#warning Kludge that counteracts the inflating of bounds for easier selection purposes - which makes reparenting worse.
            Rectangle currentGlyphBounds = currentGlyph.Bounds;
            currentGlyphBounds = new Rectangle(currentGlyphBounds.Location, currentGlyphBounds.Size);
            currentGlyphBounds.Inflate (-5, -5);
            return currentGlyphBounds;
        }

		public void MouseUp(object sender, MouseEventArgs e)
		{
			if (_Model.Header.ReadOnly)
			{
				_CurrentGlyph = null;
				return;
			}

			if (_CurrentGlyph != null)
			{
				if (_CurrentGlyph is ITransitionContactPointGlyph)
				{
					_CurrentGlyph.Parent = null;
					foreach (IGlyph glyph in _Model.Glyphs)
					{
						if (!_Model.IsStateGlyph (glyph))
						{
							continue;
						}

                        Rectangle currentGlyphBounds = GetReparentBounds(_CurrentGlyph);

						if (glyph.Bounds.Contains (currentGlyphBounds))
						{
							_CurrentGlyph.Parent = _Model.FindInnerMostChildContainingBound (glyph, currentGlyphBounds, new IsSupportedGlyphHandler (_Model.IsStateGlyph));
							break;
						}
					}
				} 
				else if (_CurrentGlyph is IPortLinkContactPointGlyph)
				{
					_CurrentGlyph.Parent = null;
					foreach (IGlyph glyph in _Model.Glyphs)
					{
						if (!_Model.IsComponentGlyph (glyph))
						{
							continue;
						}

                        Rectangle currentGlyphBounds = GetReparentBounds(_CurrentGlyph);
					    
						if (glyph.Bounds.Contains (currentGlyphBounds))
						{
							_CurrentGlyph.Parent = _Model.FindInnerMostChildContainingBound (glyph, currentGlyphBounds, new IsSupportedGlyphHandler (_Model.IsComponentGlyph));
							break;
						}
					}
				} 
				else if (_CurrentGlyph is IOperationPortLinkContactPointGlyph)
				{
					_CurrentGlyph.Parent = null;
					foreach (IGlyph glyph in _Model.Glyphs)
					{
						if (!_Model.IsOperationGlyph (glyph))
						{
							continue;
						}

						if (glyph.Bounds.Contains (_CurrentGlyph.Bounds))
						{
							IGlyph parentGlyph = _Model.FindInnerMostChildContainingBound (glyph, _CurrentGlyph.Bounds, new IsSupportedGlyphHandler (_Model.IsOperationGlyph));
							foreach (IGlyph child in parentGlyph.Children)
							{
								if (child is IOperationPortGlyph)
								{
									if (child.Bounds.Contains (_CurrentGlyph.Bounds))
									{
										_CurrentGlyph.Parent = parentGlyph;
										break;
									}
								}
							}
							break;
						}
					}
				} 
				else if (_ActualCurrentGlyph is IStateGlyph)
				{
					_Model.ReparentGlyphs (_ActualCurrentGlyph, _Model.Glyphs, new IsSupportedGlyphHandler (_Model.IsStateGlyph));
				}
				else if (_ActualCurrentGlyph is IComponentGlyph)
				{
					_Model.ReparentGlyphs (_ActualCurrentGlyph, _Model.Glyphs, new IsSupportedGlyphHandler (_Model.IsComponentGlyph));
				}
				else if (_ActualCurrentGlyph is IOperationGlyph)
				{
					_Model.ReparentGlyphs (_ActualCurrentGlyph, _Model.Glyphs, new IsSupportedGlyphHandler (_Model.IsOperationGlyph));
				}
				
				_Context.RefreshView ();
			}
			_CurrentGlyph = null;
		}

		#endregion
	}
}
