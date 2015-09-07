using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for QHsmExecutionControllerView.
	/// </summary>
	public class QHsmExecutionControllerView : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button loadHsmButton;
		private System.Windows.Forms.Button injectEventButton;
		private System.Windows.Forms.TextBox typeNameInput;
		private System.Windows.Forms.ComboBox eventInput;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox commandInput;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button executeCommandButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public QHsmExecutionControllerView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_AppDomain = AppDomain.CreateDomain (Guid.NewGuid ().ToString ());
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			try
			{
				_Controller.Dispose ();
			} 
			catch {}

			try 
			{
				if (_AppDomain != null)
				{
					AppDomain.Unload (_AppDomain);
					_AppDomain = null;
				}
			} 
			catch {}

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.typeNameInput = new System.Windows.Forms.TextBox();
			this.loadHsmButton = new System.Windows.Forms.Button();
			this.injectEventButton = new System.Windows.Forms.Button();
			this.eventInput = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.commandInput = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.executeCommandButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// typeNameInput
			// 
			this.typeNameInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.typeNameInput.Location = new System.Drawing.Point(32, 16);
			this.typeNameInput.Name = "typeNameInput";
			this.typeNameInput.Size = new System.Drawing.Size(368, 20);
			this.typeNameInput.TabIndex = 0;
			this.typeNameInput.Text = "";
			// 
			// loadHsmButton
			// 
			this.loadHsmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.loadHsmButton.Location = new System.Drawing.Point(416, 16);
			this.loadHsmButton.Name = "loadHsmButton";
			this.loadHsmButton.TabIndex = 1;
			this.loadHsmButton.Text = "&Load Hsm";
			this.loadHsmButton.Visible = false;
			this.loadHsmButton.Click += new System.EventHandler(this.loadHsmButton_Click);
			// 
			// injectEventButton
			// 
			this.injectEventButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.injectEventButton.Location = new System.Drawing.Point(416, 72);
			this.injectEventButton.Name = "injectEventButton";
			this.injectEventButton.TabIndex = 3;
			this.injectEventButton.Text = "&Inject";
			this.injectEventButton.Click += new System.EventHandler(this.injectEventButton_Click);
			// 
			// eventInput
			// 
			this.eventInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.eventInput.Location = new System.Drawing.Point(32, 80);
			this.eventInput.Name = "eventInput";
			this.eventInput.Size = new System.Drawing.Size(368, 21);
			this.eventInput.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 48);
			this.label1.Name = "label1";
			this.label1.TabIndex = 4;
			this.label1.Text = "Direct Events:";
			// 
			// commandInput
			// 
			this.commandInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.commandInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.commandInput.Location = new System.Drawing.Point(32, 152);
			this.commandInput.Name = "commandInput";
			this.commandInput.Size = new System.Drawing.Size(368, 21);
			this.commandInput.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 120);
			this.label2.Name = "label2";
			this.label2.TabIndex = 6;
			this.label2.Text = "Commands";
			// 
			// executeCommandButton
			// 
			this.executeCommandButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.executeCommandButton.Location = new System.Drawing.Point(424, 144);
			this.executeCommandButton.Name = "executeCommandButton";
			this.executeCommandButton.TabIndex = 7;
			this.executeCommandButton.Text = "Exec&ute";
			// 
			// QHsmExecutionControllerView
			// 
			this.Controls.Add(this.executeCommandButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.commandInput);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.eventInput);
			this.Controls.Add(this.injectEventButton);
			this.Controls.Add(this.typeNameInput);
			this.Controls.Add(this.loadHsmButton);
			this.Name = "QHsmExecutionControllerView";
			this.Size = new System.Drawing.Size(512, 266);
			this.ResumeLayout(false);

		}
		#endregion

		QHsmExecutionController _Controller;
		public QHsmExecutionController Controller 
		{ 
			get 
			{ 
				return _Controller;
			}
			set 
			{
				_Controller = value;
			}
		}

		public void SetMachineModel (DiagramModel model, TestAppForm appForm)
		{
			CodeCompiler compiler = new CodeCompiler ();
			System.CodeDom.Compiler.CompilerResults results = compiler.Compile (model);
			if (!results.Errors.HasErrors)
			{
				string typeName = model.Header.NameSpace + "." + model.Header.Name;
				Type type = results.CompiledAssembly.GetType (typeName);
				qf4net.ILQHsm hsm = HsmUtil.CreateHsm (type);
				Controller.Execute (hsm);
			} 
			else 
			{
				foreach (string msg in results.Output)
				{
					appForm.Log (Color.Red, msg + "\n");
				}
			}
	}

		public void SetMachineName (string name)
		{
			typeNameInput.Text = name;
			loadHsmButton_Click (this, null);
		}

		public void SetMachine (qf4net.ILQHsm hsm)
		{
			typeNameInput.Text = hsm.GetType ().FullName;
			Controller.Execute (hsm);
		}

		AppDomain _AppDomain;

		private void loadHsmButton_Click(object sender, System.EventArgs e)
		{
			string[] strlist = typeNameInput.Text.Split (',');
			if (strlist.Length != 2) 
			{
				throw new FormatException ("TypeName must be fully qualified");
			}
			string typeName = strlist [0].Trim ();
			string assemblyName = strlist [1].Trim ();
			typeName = typeName + ", " + assemblyName;
			Type type = Type.GetType (typeName);
			if (type == null)
			{
				throw new NullReferenceException ("Type [" + typeName + "] not found");
			}
			qf4net.ILQHsm hsm = HsmUtil.CreateHsm (type);
			Controller.Execute (hsm);
		}

		private void injectEventButton_Click(object sender, System.EventArgs e)
		{
			if (eventInput.Text.Trim () != "")
			{
				string inject = eventInput.Text;
				inject = inject.Trim ();
				string[] strlist = inject.Split (new char[] {','}, 2);
				string eventName = strlist[0].Trim ();
				System.Diagnostics.Debug.Assert (eventName != "");
				string data = null;
				if (strlist.Length > 1)
				{
					data = strlist [1].Trim ();
				}
				Controller.Hsm.AsyncDispatch (new qf4net.QEvent (eventName, data));

				if (!eventInput.Items.Contains (inject))
				{
					eventInput.Items.Add (inject);
				}
			}
		}
	}
}
