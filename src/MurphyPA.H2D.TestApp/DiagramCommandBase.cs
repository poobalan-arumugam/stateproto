using System;

namespace MurphyPA.H2D.TestApp
{
	using System.Windows.Forms;
	/// <summary>
	/// Summary description for DiagramCommandBase.
	/// </summary>
	public abstract class DiagramCommandBase : GuiCommandBase
	{
		IUIInterationContext _Context;
		protected IUIInterationContext Context
		{
			get 
			{
				return _Context;
			}
		}

		public DiagramCommandBase (IUIInterationContext context, Button button, MenuItem menuItem)
		{
			_Context = context;
			if (button != null)
			{
				SetButton (button);
			}
			if (menuItem != null)
			{
				SetMenuItem (menuItem);
			}
		}
	}
}
