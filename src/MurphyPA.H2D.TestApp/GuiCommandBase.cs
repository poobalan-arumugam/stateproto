using System;

namespace MurphyPA.H2D.TestApp
{
	using System.Windows.Forms;
	/// <summary>
	/// Summary description for ButtonCommandBase.
	/// </summary>
	public abstract class GuiCommandBase : ICommand
	{
		Button _Button;
		public void SetButton (Button button)
		{
			_Button = button;
			_Button.Tag = this;
			_Button.Click += new EventHandler(Do_Click);
		}

		MenuItem _MenuItem;
		public void SetMenuItem (MenuItem menuItem)
		{
			_MenuItem = menuItem;
			_MenuItem.Click += new EventHandler(Do_Click);
		}

		#region ICommand Members

		public abstract void Execute();

		#endregion

		private void Do_Click (object sender, EventArgs e)
		{
			Execute ();
		}
	}
}
