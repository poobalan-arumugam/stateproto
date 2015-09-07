using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for SaveAsImageCommand.
	/// </summary>
	public class SaveAsImageCommand : ICommand
	{
		DiagramModel _Model;
		IUIInterationContext _Context;
		string _LastFileName;

		public SaveAsImageCommand(DiagramModel model, IUIInterationContext context, string lastFileName)
		{
			_Model = model;
			_Context = context;
			_LastFileName = lastFileName;
		}

		#region ICommand Members

		public void Execute()
		{
			Rectangle bounds = _Model.GetDiagramBounds ();
			Image image = new Bitmap (bounds.Right, bounds.Bottom);
			using (Graphics graphics = Graphics.FromImage (image))
			{
				PaintEventArgs paintEv = new PaintEventArgs (graphics, bounds); 
				_Context.PaintDrawingArea (paintEv);
			}

			string fileName = "img.jpg";
			if (_LastFileName != null)
			{
				fileName = _LastFileName + ".jpg";
			}

			SaveFileDialog dialog = new SaveFileDialog ();
			dialog.FileName = fileName;
			dialog.DefaultExt = "jpg";
			dialog.Filter = "JPeg Images|*.jpg";
			//dialog.Filter = "Bitmap Images|*.bmp";
			if (dialog.ShowDialog () == DialogResult.OK)
			{
				fileName = dialog.FileName;
				image.Save (fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
			}
		}

		#endregion
	}
}
