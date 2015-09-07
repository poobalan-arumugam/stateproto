using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for StateDiagramView.
	/// </summary>
	public class StateDiagramView : System.Windows.Forms.UserControl
	{
		private MurphyPA.H2D.TestApp.TestStateGlyphControl testStateGlyphControl1;
		private MurphyPA.H2D.TestApp.GlyphPropertyWindow glyphPropertyWindow1;
		private System.Windows.Forms.Panel PropPanel;
		private System.Windows.Forms.Panel ExecutionPanel;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Splitter splitter1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StateDiagramView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		public StateDiagramView(bool propIsVisible)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			PropPanel.Visible = propIsVisible;
			ExecutionPanel.Visible = propIsVisible;
			testStateGlyphControl1.SetCommandsPanelVisible (propIsVisible);
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
			this.testStateGlyphControl1 = new MurphyPA.H2D.TestApp.TestStateGlyphControl();
			this.glyphPropertyWindow1 = new MurphyPA.H2D.TestApp.GlyphPropertyWindow();
			this.PropPanel = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.ExecutionPanel = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.PropPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// testStateGlyphControl1
			// 
			this.testStateGlyphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.testStateGlyphControl1.GlyphPropertyWindow = this.glyphPropertyWindow1;
			this.testStateGlyphControl1.Location = new System.Drawing.Point(0, 0);
			this.testStateGlyphControl1.Name = "testStateGlyphControl1";
			this.testStateGlyphControl1.Size = new System.Drawing.Size(613, 504);
			this.testStateGlyphControl1.TabIndex = 3;
			// 
			// glyphPropertyWindow1
			// 
			this.glyphPropertyWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.glyphPropertyWindow1.Glyph = null;
			this.glyphPropertyWindow1.Location = new System.Drawing.Point(0, 0);
			this.glyphPropertyWindow1.Name = "glyphPropertyWindow1";
			this.glyphPropertyWindow1.Size = new System.Drawing.Size(312, 277);
			this.glyphPropertyWindow1.TabIndex = 5;
			// 
			// PropPanel
			// 
			this.PropPanel.Controls.Add(this.glyphPropertyWindow1);
			this.PropPanel.Controls.Add(this.splitter2);
			this.PropPanel.Controls.Add(this.ExecutionPanel);
			this.PropPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.PropPanel.Location = new System.Drawing.Point(616, 0);
			this.PropPanel.Name = "PropPanel";
			this.PropPanel.Size = new System.Drawing.Size(312, 504);
			this.PropPanel.TabIndex = 6;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(0, 277);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(312, 3);
			this.splitter2.TabIndex = 7;
			this.splitter2.TabStop = false;
			// 
			// ExecutionPanel
			// 
			this.ExecutionPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ExecutionPanel.Location = new System.Drawing.Point(0, 280);
			this.ExecutionPanel.Name = "ExecutionPanel";
			this.ExecutionPanel.Size = new System.Drawing.Size(312, 224);
			this.ExecutionPanel.TabIndex = 6;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(613, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 504);
			this.splitter1.TabIndex = 7;
			this.splitter1.TabStop = false;
			// 
			// StateDiagramView
			// 
			this.Controls.Add(this.testStateGlyphControl1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.PropPanel);
			this.Name = "StateDiagramView";
			this.Size = new System.Drawing.Size(928, 504);
			this.PropPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void SetExecutionWindow (UserControl control)
		{
			ExecutionPanel.Controls.Clear ();
			ExecutionPanel.Controls.Add (control);
			control.Dock = DockStyle.Fill;
			control.Select ();
		}

		public TestStateGlyphControl StateControl { get { return testStateGlyphControl1; } }
	}
}
