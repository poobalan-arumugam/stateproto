using System;

namespace MurphyPA.H2D.TestApp
{
	using System.Windows.Forms;
	/// <summary>
	/// Summary description for LoadFileWithDialogCommand.
	/// </summary>
	public class LoadFileWithDialogCommand : DiagramCommandBase
	{
		OpenFileDialog _OpenFileDialog;
		public LoadFileWithDialogCommand (IUIInterationContext context, OpenFileDialog openFileDialog, Button button, MenuItem menuItem)
			: base (context, button, menuItem)
		{
			_OpenFileDialog = openFileDialog;
		}

		public override void Execute()
		{
			_OpenFileDialog.InitialDirectory = Environment.CurrentDirectory;
			DialogResult dialogResult = _OpenFileDialog.ShowDialog ();
			if (dialogResult == DialogResult.OK)
			{
				Context.ClearModel ();
				LoadFile (_OpenFileDialog.FileName);
				Context.ShowHeader ();
				Context.Model.Header.ReadOnly = Context.Model.HasGlyphs ();
			}
		}

		private void LoadFile (string fileName)
		{
			LoadFileCommand command = new LoadFileCommand (fileName, Context);
			command.Execute ();
		}
	}
}
