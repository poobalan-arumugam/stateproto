using System;
using System.Windows.Forms;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for SimpleStringEditor.
	/// </summary>
	public class SimpleStringEditor : qf4net.IQEventEditor
	{
		public SimpleStringEditor()
		{
		}

		#region IQEventEditor Members

		public bool SupportsParse
		{
			get
			{
				return true;
			}
		}

		public object Parse(string value)
		{
			return value;
		}

		StringEntry _StringEntry;

		public bool Edit(qf4net.IQEventEditContext context)
		{
			Form frm = context.Container as Form;
			_StringEntry = new StringEntry ();
			if (context.Instance != null)
			{
				_StringEntry.InputText = (string) context.Instance;
			}
			frm.Controls.Add (_StringEntry);
			_StringEntry.Dock = DockStyle.Fill;
			_StringEntry.Select ();
			bool ok = true;
			while (ok)
			{
				ok = context.Edit ();
				if (ok && _StringEntry.InputText.Trim () != "") 
				{
					context.Instance = _StringEntry.InputText;
					break;
				}
			}
			return ok;
		}

		#endregion
	}
}
