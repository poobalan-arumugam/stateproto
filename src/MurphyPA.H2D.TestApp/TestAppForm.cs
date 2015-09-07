using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for TestAppForm.
	/// </summary>
	public class TestAppForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel logPanel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button clearLogButton;
		private System.Windows.Forms.RichTextBox LogViewer;
		private MurphyPA.H2D.TestApp.StateDiagramView stateDiagramView;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem exitApplicationMenuItem;
		private System.Windows.Forms.MenuItem windowCascadeMenuItem;
		private System.Windows.Forms.MenuItem windowTileVerticalMenuItem;
		private System.Windows.Forms.MenuItem windowTileHorizontalMenuItem;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem newStateChartDiagramMenuItem;
		private System.Windows.Forms.MenuItem toolsMenuItem;
		private System.Windows.Forms.MenuItem publishUpdateMenuItem;
		private System.Windows.Forms.MenuItem updateApplicationMenuItem;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TestAppForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			AddChild ("Log", "Log", logPanel);
			AddChild ("NoName!", "NoName!", stateDiagramView);

			InitToolMenu ();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TestAppForm));
            this.logPanel = new System.Windows.Forms.Panel();
            this.LogViewer = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.stateDiagramView = new MurphyPA.H2D.TestApp.StateDiagramView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.exitApplicationMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.newStateChartDiagramMenuItem = new System.Windows.Forms.MenuItem();
            this.toolsMenuItem = new System.Windows.Forms.MenuItem();
            this.publishUpdateMenuItem = new System.Windows.Forms.MenuItem();
            this.updateApplicationMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.windowCascadeMenuItem = new System.Windows.Forms.MenuItem();
            this.windowTileVerticalMenuItem = new System.Windows.Forms.MenuItem();
            this.windowTileHorizontalMenuItem = new System.Windows.Forms.MenuItem();
            this.logPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logPanel
            // 
            this.logPanel.Controls.Add(this.LogViewer);
            this.logPanel.Controls.Add(this.panel1);
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.Location = new System.Drawing.Point(784, 0);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(216, 558);
            this.logPanel.TabIndex = 5;
            // 
            // LogViewer
            // 
            this.LogViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogViewer.Location = new System.Drawing.Point(0, 56);
            this.LogViewer.Name = "LogViewer";
            this.LogViewer.Size = new System.Drawing.Size(216, 502);
            this.LogViewer.TabIndex = 2;
            this.LogViewer.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.clearLogButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(216, 56);
            this.panel1.TabIndex = 1;
            // 
            // clearLogButton
            // 
            this.clearLogButton.Location = new System.Drawing.Point(16, 16);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.TabIndex = 0;
            this.clearLogButton.Text = "&Clear";
            this.clearLogButton.Click += new System.EventHandler(this.clearLogButton_Click);
            // 
            // stateDiagramView
            // 
            this.stateDiagramView.Dock = System.Windows.Forms.DockStyle.Left;
            this.stateDiagramView.Location = new System.Drawing.Point(0, 0);
            this.stateDiagramView.Name = "stateDiagramView";
            this.stateDiagramView.Size = new System.Drawing.Size(784, 558);
            this.stateDiagramView.TabIndex = 6;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.menuItem1,
                                                                                      this.menuItem5,
                                                                                      this.toolsMenuItem,
                                                                                      this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.exitApplicationMenuItem});
            this.menuItem1.Text = "&File";
            // 
            // exitApplicationMenuItem
            // 
            this.exitApplicationMenuItem.Index = 0;
            this.exitApplicationMenuItem.Text = "E&xit";
            this.exitApplicationMenuItem.Click += new System.EventHandler(this.exitApplicationMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.newStateChartDiagramMenuItem});
            this.menuItem5.Text = "Dia&gram";
            // 
            // newStateChartDiagramMenuItem
            // 
            this.newStateChartDiagramMenuItem.Index = 0;
            this.newStateChartDiagramMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftJ;
            this.newStateChartDiagramMenuItem.Text = "&New StateChart Diagram";
            this.newStateChartDiagramMenuItem.Click += new System.EventHandler(this.newStateChartDiagramMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.Index = 2;
            this.toolsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                          this.publishUpdateMenuItem,
                                                                                          this.updateApplicationMenuItem});
            this.toolsMenuItem.Text = "&Tools";
            // 
            // publishUpdateMenuItem
            // 
            this.publishUpdateMenuItem.Index = 0;
            this.publishUpdateMenuItem.Text = "&Publish Update";
            this.publishUpdateMenuItem.Click += new System.EventHandler(this.publishUpdateMenuItem_Click);
            // 
            // updateApplicationMenuItem
            // 
            this.updateApplicationMenuItem.Index = 1;
            this.updateApplicationMenuItem.Text = "&Update this Application";
            this.updateApplicationMenuItem.Click += new System.EventHandler(this.updateApplicationMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            this.menuItem3.MdiList = true;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.windowCascadeMenuItem,
                                                                                      this.windowTileVerticalMenuItem,
                                                                                      this.windowTileHorizontalMenuItem});
            this.menuItem3.Text = "&Window";
            // 
            // windowCascadeMenuItem
            // 
            this.windowCascadeMenuItem.Index = 0;
            this.windowCascadeMenuItem.Text = "C&ascade";
            this.windowCascadeMenuItem.Click += new System.EventHandler(this.windowCascadeMenuItem_Click);
            // 
            // windowTileVerticalMenuItem
            // 
            this.windowTileVerticalMenuItem.Index = 1;
            this.windowTileVerticalMenuItem.Text = "Tile &Vertical";
            this.windowTileVerticalMenuItem.Click += new System.EventHandler(this.windowTileVerticalMenuItem_Click);
            // 
            // windowTileHorizontalMenuItem
            // 
            this.windowTileHorizontalMenuItem.Index = 2;
            this.windowTileHorizontalMenuItem.Text = "Tile &Horizontal";
            this.windowTileHorizontalMenuItem.Click += new System.EventHandler(this.windowTileHorizontalMenuItem_Click);
            // 
            // TestAppForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1000, 558);
            this.Controls.Add(this.logPanel);
            this.Controls.Add(this.stateDiagramView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Menu = this.mainMenu1;
            this.Name = "TestAppForm";
            this.Text = "StateChartProto";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.TestAppForm_Closing);
            this.Load += new System.EventHandler(this.TestAppForm_Load);
            this.logPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		Hashtable _ExecutionModelTabs = new Hashtable ();

		internal protected void AddChild (string id, string name, Control control)
		{
			Form form = new Form ();
			form.MdiParent = this;
			form.Controls.Add (control);
			control.Dock = DockStyle.Fill;
			form.Text = name;
			form.WindowState = FormWindowState.Maximized;
			form.Show ();

			if (IsNotBootstrapControl (control))
			{
				_ExecutionModelTabs.Add (id, form);
			}
		}

		bool IsNotBootstrapControl (Control control)
		{
			return (control != stateDiagramView) && (control != logPanel);
		}

		public void ClearExecutionModelTabs ()
		{
			foreach (DictionaryEntry de in _ExecutionModelTabs)
			{
				Form form = de.Value as Form;
				form.Close ();
			}
			_ExecutionModelTabs.Clear ();
			stateDiagramView.Parent.Focus ();
		}

		public void RegisterHsm (qf4net.ILQHsm hsm)
		{
			hsm.EventManager.PolledEvent += new qf4net.PolledEventHandler(EventManager_PolledEvent);
			hsm.StateChange += new EventHandler(hsm_StateChange);
			hsm.DispatchException += new qf4net.DispatchExceptionHandler(hsm_DispatchException);
			hsm.UnhandledTransition += new qf4net.DispatchUnhandledTransitionHandler(hsm_UnhandledTransition);
		}

		public void UnregisterHsm (qf4net.ILQHsm hsm)
		{
			hsm.EventManager.PolledEvent -= new qf4net.PolledEventHandler(EventManager_PolledEvent);
			hsm.StateChange -= new EventHandler(hsm_StateChange);
			hsm.DispatchException -= new qf4net.DispatchExceptionHandler(hsm_DispatchException);
			hsm.UnhandledTransition -= new qf4net.DispatchUnhandledTransitionHandler(hsm_UnhandledTransition);
		}

		private void _WriterWriteLine (Color color, string fmt, params object[] args)
		{
			string msg = fmt;
			if (args != null && args.Length > 0)
			{
				msg = string.Format (fmt, args);
			}

			Log (color, msg + "\n");
		}

		LogHandler _Log = null;
		System.IO.StreamWriter _LogFile;

		string GetLogFileName ()
		{
			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly ();
			string dirLine = entryAssembly.Location;
			string directory = System.IO.Path.GetDirectoryName (dirLine);
			string fileName = System.IO.Path.Combine (directory, "StateLog.txt");
			return fileName;
		}

		protected System.IO.StreamWriter FileWriter 
		{
			get 
			{
				if (_LogFile == null)
				{
					string fileName = GetLogFileName ();
					_LogFile = System.IO.File.AppendText (fileName);
					_LogFile.WriteLine ("------------------------------------ {0} ---------------------------", DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss.fff"));
				}
				return _LogFile;
			}
		}

		public void Log (Color foreColor, string fmt, params object[] args)
		{
			InternalLog (foreColor, fmt, args);
		}

        protected string LogMessage (string fmt, params object[] args)
        {
            string msg = fmt;
            if (args != null && args.Length > 0)
            {
                msg = string.Format (fmt, args);
            }
            return msg;
        }

		protected void InternalLog (Color foreColor, string fmt, params object[] args)
		{
			if (LogViewer.InvokeRequired)
			{
				if (_Log == null)
				{
					_Log = new LogHandler (InternalLog);
				}

				LogViewer.Invoke (_Log, new object [] {foreColor, fmt, args});
			} 
			else 
			{
                string msg = LogMessage (fmt, args);
                msg = DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss.fff") + " " + msg;

				FileWriter.Write (msg);
				FileWriter.Flush ();

				LogViewer.SelectionLength = 0;
				int start = LogViewer.TextLength;
				LogViewer.AppendText (msg);
				LogViewer.Select (start, msg.Length);
				LogViewer.SelectionColor = foreColor;
			}
		}

		void _WriterInc () {}
		void _WriterDec () {}

		private void hsm_StateChange(object sender, EventArgs e)
		{
			qf4net.LogStateEventArgs args = e as qf4net.LogStateEventArgs;
			qf4net.ILQHsm hsm = sender as qf4net.ILQHsm;

			switch (args.LogType)
			{
				case qf4net.StateLogType.Exit:	_WriterDec(); break;		
			}

			switch (args.LogType)
			{
				case qf4net.StateLogType.Init: 
				{
					_WriterWriteLine (Color.YellowGreen, "[{0}-{1}] {2} to {3}", hsm, args.LogType, args.State.Method.Name, args.NextState.Method.Name);					
				} break;
				case qf4net.StateLogType.Entry:
				{
					_WriterWriteLine (Color.Green, "[{0}-{1}] {2}", hsm, args.LogType, args.State.Method.Name);
				} break;
				case qf4net.StateLogType.Exit:			
				{
					_WriterWriteLine (Color.Goldenrod, "[{0}-{1}] {2}", hsm, args.LogType, args.State.Method.Name);
				} break;
				case qf4net.StateLogType.EventTransition: 
				{
					_WriterWriteLine (Color.DarkBlue, "[{0}-{1}] {2} on {3} to {4} -> {5}", hsm, args.LogType, args.State.Method.Name, args.EventName, args.NextState.Method.Name, args.EventDescription);					
				} break;
				case qf4net.StateLogType.Log: 
				{
					_WriterWriteLine (Color.Brown, "[{0}-{1}] {2}", hsm, args.LogType, args.LogText);					
				} break;
				default: throw new NotSupportedException ("StateLogType." + args.LogType.ToString ());
			}

			switch (args.LogType)
			{
				case qf4net.StateLogType.Entry: _WriterInc (); break;
			}
		}

		private void hsm_DispatchException(Exception ex, qf4net.IQHsm hsm, System.Reflection.MethodInfo stateMethod, qf4net.IQEvent ev)
		{
			_WriterWriteLine (Color.Red, "[{0}] Exception: on event {1}\n{2}", hsm, ev, ex.ToString ());
		}

		private void clearLogButton_Click(object sender, System.EventArgs e)
		{
			LogViewer.Clear ();
		}

		private void EventManager_PolledEvent(qf4net.IQEventManager eventManager, qf4net.IQHsm hsm, qf4net.IQEvent ev, qf4net.PollContext pollContext)
		{
			//_WriterWriteLine (Color.Gray, "Ev: {0} {1} {2} {3}", pollContext, ev.GetHashCode (), ev.QSignal, ev.QData);
		}

		private void hsm_UnhandledTransition(qf4net.IQHsm hsm, System.Reflection.MethodInfo stateMethod, qf4net.IQEvent ev)
		{
			_WriterWriteLine (Color.Crimson, "[{0}] Unhandled Event: {1}", hsm, ev);
		}

		private void TestAppForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach (Control control in this.MdiChildren)
			{
				if (control is StateDiagramView)
				{
					StateDiagramView view = control as StateDiagramView;
					if (view.StateControl.Model.IsDirty)
					{
						view.StateControl.SaveFile ();
						if (view.StateControl.Model.IsDirty)
						{
							string q1 = "State App: Are you sure you want to quit?";
							string q2 = "The diagram " + view.StateControl.Model.Header.Name + " has been modified\nbut has not been saved.";
							DialogResult result = MessageBox.Show (this, q2, q1, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
							e.Cancel = result == DialogResult.No;
							if (e.Cancel) 
							{
								break;
							}
						}
					}
				}
			}
		}

		private void exitApplicationMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Close ();
		}

		private void windowCascadeMenuItem_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi (MdiLayout.Cascade);
		}

		private void windowTileVerticalMenuItem_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi (MdiLayout.TileVertical);
		}

		private void windowTileHorizontalMenuItem_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi (MdiLayout.TileHorizontal);
		}

		private void newStateChartDiagramMenuItem_Click(object sender, System.EventArgs e)
		{
			StateDiagramView view = new StateDiagramView ();
			AddChild (Guid.NewGuid ().ToString (), "Noname", view);
		}

		private void TestAppForm_Load(object sender, System.EventArgs e)
		{
			string nextIs = "";
			foreach (string arg in Environment.GetCommandLineArgs ())
			{
				if ("-i" == nextIs)
				{
					stateDiagramView.StateControl.LoadFile (arg);
					nextIs = "";
				}
				if ("-i" == arg)
				{
					nextIs = "-i";
				}
			}
		}

		protected bool DevEnvironentIsRunning ()
		{
			System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName ("devenv");

			string userName = System.Security.Principal.WindowsIdentity.GetCurrent ().Name;
			bool isDeveloperUser = userName.IndexOf ("murphy") != -1;
			return isDeveloperUser && processes != null && processes.Length > 0;
		}

		protected void InitToolMenu ()
		{
			publishUpdateMenuItem.Visible = DevEnvironentIsRunning ();
		}

		private void publishUpdateMenuItem_Click(object sender, System.EventArgs e)
		{
            // not including StepByStep.Deployment for now...
#if IncludeDeployment		
			new StepByStep.Deployment.SoftwareUpdates.UserInterface.PublishUpdateCmd ().Execute ();		
#endif
		}

		private void updateApplicationMenuItem_Click(object sender, System.EventArgs e)
		{
#if IncludeDeployment
			new StepByStep.Deployment.SoftwareUpdates.UserInterface.DownloadUpdateCmd ().Execute ();		
#endif
		}
	}
}
