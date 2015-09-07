using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ProcessComponentFrameExecutionView.
	/// </summary>
	public class ProcessComponentFrameExecutionView : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ComboBox hsmListInput;
		private System.Windows.Forms.Button injectButton;
		private System.Windows.Forms.ComboBox eventInput;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox commandInput;
		private System.Windows.Forms.Button commandExecuteButton;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProcessComponentFrameExecutionView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			InitEditors ();

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
			this.hsmListInput = new System.Windows.Forms.ComboBox();
			this.injectButton = new System.Windows.Forms.Button();
			this.eventInput = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.commandInput = new System.Windows.Forms.ComboBox();
			this.commandExecuteButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// hsmListInput
			// 
			this.hsmListInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.hsmListInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.hsmListInput.Location = new System.Drawing.Point(16, 48);
			this.hsmListInput.Name = "hsmListInput";
			this.hsmListInput.Size = new System.Drawing.Size(416, 21);
			this.hsmListInput.TabIndex = 0;
			this.hsmListInput.SelectedIndexChanged += new System.EventHandler(this.hsmListInput_SelectedIndexChanged);
			// 
			// injectButton
			// 
			this.injectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.injectButton.Location = new System.Drawing.Point(360, 120);
			this.injectButton.Name = "injectButton";
			this.injectButton.TabIndex = 2;
			this.injectButton.Text = "&Inject";
			this.injectButton.Click += new System.EventHandler(this.injectButton_Click);
			// 
			// eventInput
			// 
			this.eventInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.eventInput.Location = new System.Drawing.Point(16, 120);
			this.eventInput.Name = "eventInput";
			this.eventInput.Size = new System.Drawing.Size(328, 21);
			this.eventInput.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 3;
			this.label1.Text = "State Model";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 88);
			this.label2.Name = "label2";
			this.label2.TabIndex = 4;
			this.label2.Text = "Input Events";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 160);
			this.label3.Name = "label3";
			this.label3.TabIndex = 5;
			this.label3.Text = "Input Commands";
			// 
			// commandInput
			// 
			this.commandInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.commandInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.commandInput.Location = new System.Drawing.Point(24, 192);
			this.commandInput.Name = "commandInput";
			this.commandInput.Size = new System.Drawing.Size(320, 21);
			this.commandInput.TabIndex = 6;
			// 
			// commandExecuteButton
			// 
			this.commandExecuteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.commandExecuteButton.Location = new System.Drawing.Point(360, 192);
			this.commandExecuteButton.Name = "commandExecuteButton";
			this.commandExecuteButton.Size = new System.Drawing.Size(72, 23);
			this.commandExecuteButton.TabIndex = 7;
			this.commandExecuteButton.Text = "Exec&ute";
			this.commandExecuteButton.Click += new System.EventHandler(this.commandExecuteButton_Click);
			// 
			// ProcessComponentFrameExecutionView
			// 
			this.Controls.Add(this.commandExecuteButton);
			this.Controls.Add(this.commandInput);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.eventInput);
			this.Controls.Add(this.injectButton);
			this.Controls.Add(this.hsmListInput);
			this.Name = "ProcessComponentFrameExecutionView";
			this.Size = new System.Drawing.Size(448, 256);
			this.ResumeLayout(false);

		}
		#endregion

		ProcessComponentFrame _ComponentFrame;
		public void Init (ProcessComponentFrame componentFrame)
		{
			_ComponentFrame = componentFrame;
			ArrayList list = new ArrayList ();
			foreach (DictionaryEntry de in _ComponentFrame.ComponentContexts)
			{
				list.Add (de.Key.ToString ());
			}
			hsmListInput.DataSource = list;
		}

		qf4net.ILQHsm SelectedHSM
		{
			get 
			{
				qf4net.ILQHsm hsm = null;

				string hsmName = hsmListInput.SelectedItem as string;
				if (_ComponentFrame.ComponentContexts.Contains (hsmName))
				{
					ProcessComponentFrame.ComponentContext ctx = _ComponentFrame.ComponentContexts [hsmName] as ProcessComponentFrame.ComponentContext;
					hsm = ctx.Hsm;
				}

				return hsm;
			}
		}

		protected Hashtable _Editors = new Hashtable ();

		protected string _editorsFileName = "editors.txt";

		string GetEditorsFileName ()
		{
			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly ();
			string dirLine = entryAssembly.Location;
			string directory = System.IO.Path.GetDirectoryName (dirLine);
			string fileName = System.IO.Path.Combine (directory, _editorsFileName);
			return fileName;
		}

		protected void InitEditors ()
		{
			_Editors.Add (typeof (string), new EditorAttribute (typeof (SimpleStringEditor), typeof (qf4net.IQEventEditor)));

			string editorsFileName = GetEditorsFileName ();
			if (System.IO.File.Exists (editorsFileName))
			{
				using (System.IO.StreamReader sr = new System.IO.StreamReader (editorsFileName))
				{
					while (sr.Peek () != -1)
					{
						string line = sr.ReadLine ().Trim ();
						string[] strList = line.Split (new char[] {';'}, 2);
						if (strList.Length != 2)
						{
							throw new ArgumentException ("[" + line + "] does not contain a typename;editorname pair");
						}
						string typeName = strList [0].Trim ();
						string editorName = strList [1].Trim ();
						Type type = Type.GetType (typeName);
						Type editorType = Type.GetType (editorName);
						EditorAttribute attr = new EditorAttribute (editorType, typeof (qf4net.IQEventEditor));
						_Editors [type] = attr;
					}
				}
			}
		}

		protected EditorAttribute GetEditorType (qf4net.TransitionEventAttribute te)
		{
			if (_Editors.Contains (te.DataType))
			{
				return _Editors [te.DataType] as EditorAttribute;
			}
			foreach (EditorAttribute editor in te.DataType.GetCustomAttributes (typeof (EditorAttribute), false))
			{
				return editor;
			}
			throw new NotSupportedException (te.ToString () + " TransitionEventAttribute does not have a supported Editor");
		}

		protected qf4net.IQEventEditor GetEditor (qf4net.TransitionEventAttribute te)
		{
			EditorAttribute editor = GetEditorType (te);
			Type editorType = Type.GetType (editor.EditorTypeName);
			object editorObj = Activator.CreateInstance (editorType);
			qf4net.IQEventEditor eventEditor = editorObj as qf4net.IQEventEditor;
			if (eventEditor == null)
			{
				throw new ArgumentException (editor.EditorTypeName + " is not a valid IQEventEditor Type");
			}
			return eventEditor;
		}

		private void injectButton_Click(object sender, System.EventArgs e)
		{
			qf4net.ILQHsm hsm = SelectedHSM;
			if (hsm == null) 
			{
				throw new ArgumentException ("No hsm selected");
			}

			if (eventInput.Text.Trim () != "")
			{
				string inject = eventInput.Text;
				inject = inject.Trim ();

				// if inject matches a TransitionEventAttribute then find its editor an edit!
				foreach (qf4net.TransitionEventAttribute te in hsm.TransitionEvents)
				{
					if (te.HasDataType)
					{
						if (te.ToString () == inject)
						{
							qf4net.IQEventEditor eventEditor = GetEditor (te);

							Form frm = new OkCancelForm ();
							frm.Text = "Event " + te.EventName;
							qf4net.QEventDefaultEditContext ctx = new qf4net.QEventDefaultEditContext (frm);
							if (eventEditor.Edit (ctx) == false)
							{
								return;
							}

							hsm.AsyncDispatch (new qf4net.QEvent (te.EventName, ctx.Instance));

							if (eventEditor.SupportsParse)
							{
								inject = te + "|" + ctx.Instance;
								if (!eventInput.Items.Contains (inject))
								{
									eventInput.Items.Add (inject);
								}
							}

							return;
						}
					}
				}

				// if inject matches a TransitionEventAttribute + extra data then parse the extra data!
				foreach (qf4net.TransitionEventAttribute te in hsm.TransitionEvents)
				{
					if (te.HasDataType)
					{
						if (inject.StartsWith (te.ToString () + "|"))
						{
							qf4net.IQEventEditor eventEditor = GetEditor (te);
							if (!eventEditor.SupportsParse)
							{
								throw new NotSupportedException (te.ToString () + " editor does not support string parsing");
							}

							string[] strlist = inject.Split (new char[] {'|'}, 2);
							string eventName = strlist[0].Trim ();
							System.Diagnostics.Debug.Assert (eventName != "" && eventName == te.ToString ());
							string data = null;
							if (strlist.Length > 1)
							{
								data = strlist [1].Trim ();
							}
							if (data == null)
							{
								throw new ArgumentException ("[" + inject + "] does not contain a Parsable event data string");
							}

							object instance = eventEditor.Parse (data);

							hsm.AsyncDispatch (new qf4net.QEvent (te.EventName, instance));

							return;
						}
					}
				}

				hsm.AsyncDispatch (new qf4net.QEvent (inject));
			}
		}

		private void hsmListInput_SelectedIndexChanged_update_EventInput(object sender, System.EventArgs e)
		{
			qf4net.ILQHsm hsm = SelectedHSM;

			eventInput.Text = "";
			eventInput.Items.Clear ();
			if (hsm == null) 
			{
				return;
			}

			qf4net.TransitionEventAttribute[] transitionEventAttributes = hsm.TransitionEvents;
			foreach (qf4net.TransitionEventAttribute te in transitionEventAttributes)
			{
				eventInput.Items.Add (te);
			}
		}

		private void hsmListInput_SelectedIndexChanged_update_CommandInput(object sender, System.EventArgs e)
		{
			qf4net.ILQHsm hsm = SelectedHSM;

			commandInput.Text = "";
			commandInput.Items.Clear ();

			if (hsm == null) 
			{
				return;
			}

#warning Add command filling code...

		}

		private void hsmListInput_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			hsmListInput_SelectedIndexChanged_update_EventInput (sender, e);
			hsmListInput_SelectedIndexChanged_update_CommandInput (sender, e);
		}

		private void commandExecuteButton_Click(object sender, System.EventArgs e)
		{
			;
		}
	}
}
