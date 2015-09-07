using System;

namespace MurphyPA.H2D.TestApp
{
	using System.Windows.Forms;
	/// <summary>
	/// Summary description for LoadFileCommand.
	/// </summary>
	public class LoadFileCommand : DiagramCommandBase
	{
		string _FileName;
		public LoadFileCommand (string fileName, IUIInterationContext context)
			: base (context, null, null)
		{
			_FileName = fileName;
		}

		public override void Execute()
		{
			LoadGlyphDataFile loadFile = new LoadGlyphDataFile ();
			loadFile.Load (_FileName);
			DiagramModel model = new DiagramModel (loadFile.Header, loadFile.Glyphs);
			Context.ReplaceModel (model);
			Context.LastFileName = _FileName;

			Context.RefreshView ();
		}

	}
}
