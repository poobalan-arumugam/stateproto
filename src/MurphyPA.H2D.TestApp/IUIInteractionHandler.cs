using System;
using System.Windows.Forms;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for IInteractionHandler.
	/// </summary>
	public interface IUIInteractionHandler
	{
		void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e);
		void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e);
		void MouseUp(object sender, System.Windows.Forms.MouseEventArgs e);
		void KeyUp (object sender, System.Windows.Forms.KeyEventArgs e);

		void Draw (IGraphicsContext gc);
	}
}
