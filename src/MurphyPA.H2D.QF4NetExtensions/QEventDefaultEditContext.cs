using System;

namespace qf4net
{
	/// <summary>
	/// Summary description for QEventDefaultEditContext.
	/// </summary>
	public class QEventDefaultEditContext : IQEventEditContext
	{
		public QEventDefaultEditContext(System.ComponentModel.IComponent container)
		{
			_Container = container;
		}

		protected System.Windows.Forms.Form Form 
		{ 
			get { return _Container as System.Windows.Forms.Form; }
		}

		#region IQEventEditContext Members

		System.ComponentModel.IComponent _Container;
		public System.ComponentModel.IComponent Container
		{
			get
			{
				return _Container;
			}
		}

		object _Instance;
		public object Instance
		{
			get
			{
				return _Instance;
			}
			set
			{
				_Instance = value;
			}
		}

		public bool Edit ()
		{
			System.Windows.Forms.Form frm = Form;
			System.Windows.Forms.DialogResult result = frm.ShowDialog ();
			return result == System.Windows.Forms.DialogResult.OK;
		}

		#endregion
	}
}
