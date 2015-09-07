using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for UIGlyphInteractionHandlerBase.
	/// </summary>
	public class UIGlyphInteractionHandlerBase
	{
		protected DiagramModel _Model { get { return _Context.Model; } }
		protected IUIInterationContext _Context;


		public UIGlyphInteractionHandlerBase(IUIInterationContext context)
		{
			_Context = context;
		}

		public virtual void KeyUp (object sender, System.Windows.Forms.KeyEventArgs e) {}

		public virtual void Draw (IGraphicsContext gc) {}

		protected bool IsControlKey (System.Windows.Forms.KeyEventArgs e, System.Windows.Forms.Keys key)
		{
			bool isControl = (e.Modifiers & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control;
			if (isControl && e.KeyCode == key)
			{
				return true;
			}
			return false;
		}

	}
}
