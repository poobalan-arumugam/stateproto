using System;

namespace MurphyPA.H2D.TestApp
{
	using System.Windows.Forms;
	using System.IO;
	/// <summary>
	/// Summary description for ConvertToXmlWithSaveDialogCommand.
	/// </summary>
	public class ConvertToXmlWithSaveDialogCommand : DiagramCommandBase
	{
		SaveFileDialog _SaveFileDialog;
		public ConvertToXmlWithSaveDialogCommand (IUIInterationContext context, SaveFileDialog saveFileDialog, Button button, MenuItem menuItem)
			: base (context, button, menuItem)
		{
			_SaveFileDialog = saveFileDialog;
		}

		public override void Execute()
		{
			ConvertToXml convert = new ConvertToXml (Context.Model);

			_SaveFileDialog.FileName = GetXmlFileNameFor (Context.Model);
			DialogResult dialogResult = _SaveFileDialog.ShowDialog ();
			if (dialogResult == DialogResult.OK)
			{
				string fileName = _SaveFileDialog.FileName;
				string genFileName = fileName + ".generated";
				string text = null;

				bool ok = false;
				text = convert.Convert ();
				ok = true;

				if (ok && text != null)
				{
					using (StreamWriter sw = new StreamWriter (genFileName))
					{
						sw.WriteLine (text);
					}

					if (File.Exists (fileName))
					{
						File.Delete (fileName);
					}
					File.Move (genFileName, fileName);
					SaveXmlFileMapping (Context.Model, fileName);
				} 
				else 
				{
					MessageBox.Show ("XmlFile was not saved", "Cannot save generated xml file.");
				}
			}
		}

		string _mappingFileName = "Model2XmlFileMapping.txt";

		string GetMappingFileName ()
		{
			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly ();
			string dirLine = entryAssembly.Location;
			string directory = Path.GetDirectoryName (dirLine);
			string fileName = Path.Combine (directory, _mappingFileName);
			return fileName;
		}

		private string GetXmlFileNameFor (DiagramModel model)
		{
			string lastMatchingLine = null;
			string mappingFileName = GetMappingFileName ();
			if (File.Exists (mappingFileName))
			{
				using (StreamReader sr = File.OpenText (mappingFileName))
				{
					while (sr.Peek () != -1)
					{
						string line = sr.ReadLine ();
						if (line.StartsWith (model.Header.ModelGuid))
						{
							lastMatchingLine = line;
						}
					}
				}
			}
			if (lastMatchingLine != null)
			{
				string[] strList = lastMatchingLine.Split (',');
				System.Diagnostics.Debug.Assert (strList.Length == 4);
				string fileName = strList [1];
				return fileName;
			}
			return model.Header.Name;
		}

		protected void SaveXmlFileMapping (DiagramModel model, string xmlFileName)
		{
			using (StreamWriter sw = File.AppendText (GetMappingFileName ()))
			{
				sw.WriteLine ("{0}, {1}, {2}, {3}", model.Header.ModelGuid, xmlFileName, model.Header.Name, DateTime.Now);
			}
		}
	}
}
