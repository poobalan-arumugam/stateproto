using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for StringEntry.
	/// </summary>
	public class StringEntry : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox inputText;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StringEntry()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.inputText = new System.Windows.Forms.TextBox();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.SuspendLayout();
			// 
			// inputText
			// 
			this.inputText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.inputText.Location = new System.Drawing.Point(8, 24);
			this.inputText.Name = "inputText";
			this.inputText.Size = new System.Drawing.Size(384, 20);
			this.inputText.TabIndex = 0;
			this.inputText.Text = "";
			this.inputText.Validating += new System.ComponentModel.CancelEventHandler(this.inputText_Validating);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// StringEntry
			// 
			this.Controls.Add(this.inputText);
			this.Name = "StringEntry";
			this.Size = new System.Drawing.Size(408, 80);
			this.ResumeLayout(false);

		}
		#endregion

		private void inputText_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			errorProvider1.SetError(inputText, "");
			if (inputText.Text.Trim () == "")
			{
				errorProvider1.SetError (inputText, "Entry cannot be empty");
				e.Cancel = true;
			}
		}

		public string InputText { get { return inputText.Text; } set { inputText.Text = value; } }
	}
}
