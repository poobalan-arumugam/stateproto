using System;
using System.Drawing;
using System.Windows.Forms;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for IUIInterationContext.
	/// </summary>
	public interface IUIInterationContext
	{
		DiagramModel Model { get; }

		void SelectGlyph (IGlyph glyph);
		void RefreshView ();
		void ShowHeader ();

		void PaintDrawingArea (PaintEventArgs args);
		TestAppForm AppForm ();
		void RefreshView (object sender, EventArgs args);
		StateDiagramView ParentStateDiagramView { get; }

		string LastFileName { get; set; }
		void ReplaceModel (DiagramModel model);
		void ClearModel ();
	}
}
