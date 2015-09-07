using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using MurphyPA.H2D.Interfaces;
using System.IO;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TestStateGlyphControl : System.Windows.Forms.UserControl, IUIInterationContext
	{
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private DrawingSurface drawingPanel;
		private System.Windows.Forms.Button convertToCodebutton;
		private System.Windows.Forms.SaveFileDialog saveFileDialog2;
		private System.Windows.Forms.Button execButton;
		private System.Windows.Forms.Panel pictureHolderPanel;
		private System.Windows.Forms.Panel commandsPanel;
		private System.Windows.Forms.Timer checkVarsTimer;
		private System.Windows.Forms.GroupBox panel1;
		private System.Windows.Forms.GroupBox diagramCommandsPanel;
		private System.Windows.Forms.Button newButton;
		private System.Windows.Forms.Button moveButton;
		private System.Windows.Forms.Button bandButton;
		private System.ComponentModel.IContainer components;

		ICommand _ConvertToCode;
		ICommand _ConvertToXml;
		ICommand _LoadFile;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem loadFileMenuItem;
		private System.Windows.Forms.MenuItem saveFileMenuItem;
		private System.Windows.Forms.MenuItem newModelMenuItem;
		private System.Windows.Forms.MenuItem convertToCodeMenuItem;
		private System.Windows.Forms.MenuItem executeModelMenuItem;
		private System.Windows.Forms.MenuItem bandSelectorMenuItem;
		private System.Windows.Forms.MenuItem handSelectorMenuItem;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button toXmlButton;
		private System.Windows.Forms.SaveFileDialog saveFileDialog3;
		private System.Windows.Forms.MenuItem convertToXmlMenuItem;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.ImageList imageListTools;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox resetToHandCheckBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageStateTools;
        private System.Windows.Forms.ToolBar toolBarStateTools;
        private System.Windows.Forms.TabPage tabPageComponentTools;
        private System.Windows.Forms.ToolBar toolBarComponentTools;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton operationLinkButton;
        private System.Windows.Forms.RadioButton operationButton;
        private System.Windows.Forms.ToolBarButton toolBarButtonStateTool;
        private System.Windows.Forms.ToolBarButton toolBarButtonTransitionTool;
        private System.Windows.Forms.ToolBarButton toolBarButtonPortTool;
        private System.Windows.Forms.ToolBarButton toolBarButtonComponentTool;
        private System.Windows.Forms.ToolBarButton toolBarButtonPortLinkTool;
        private System.Windows.Forms.TabPage tabPageOriginalOperationTools;
		ICommand _SaveFile;
		public TestStateGlyphControl ()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			_ConvertToCode = new ConvertToCodeWithSaveDialogCommand (this, saveFileDialog2, convertToCodebutton, convertToCodeMenuItem);
			_LoadFile = new LoadFileWithDialogCommand (this, openFileDialog1, loadButton, loadFileMenuItem);
			_SaveFile = new SaveFileWithDialogCommand (this, saveFileDialog1, saveButton, saveFileMenuItem);
			_ConvertToXml = new ConvertToXmlWithSaveDialogCommand (this, saveFileDialog3, toXmlButton, convertToXmlMenuItem);
			CheckDefaultArgs ();


            SetScrollBars ();
        }

		GlyphPropertyWindow _GlyphPropertyWindow;
		public GlyphPropertyWindow GlyphPropertyWindow { get { return _GlyphPropertyWindow; } set { _GlyphPropertyWindow = value; } }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TestStateGlyphControl));
            this.saveButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.loadButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.newButton = new System.Windows.Forms.Button();
            this.drawingPanel = new MurphyPA.H2D.TestApp.DrawingSurface();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.newModelMenuItem = new System.Windows.Forms.MenuItem();
            this.loadFileMenuItem = new System.Windows.Forms.MenuItem();
            this.saveFileMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.convertToCodeMenuItem = new System.Windows.Forms.MenuItem();
            this.executeModelMenuItem = new System.Windows.Forms.MenuItem();
            this.convertToXmlMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.bandSelectorMenuItem = new System.Windows.Forms.MenuItem();
            this.handSelectorMenuItem = new System.Windows.Forms.MenuItem();
            this.commandsPanel = new System.Windows.Forms.Panel();
            this.diagramCommandsPanel = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageStateTools = new System.Windows.Forms.TabPage();
            this.toolBarStateTools = new System.Windows.Forms.ToolBar();
            this.toolBarButtonStateTool = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonTransitionTool = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonPortTool = new System.Windows.Forms.ToolBarButton();
            this.imageListTools = new System.Windows.Forms.ImageList(this.components);
            this.tabPageComponentTools = new System.Windows.Forms.TabPage();
            this.toolBarComponentTools = new System.Windows.Forms.ToolBar();
            this.toolBarButtonComponentTool = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonPortLinkTool = new System.Windows.Forms.ToolBarButton();
            this.tabPageOriginalOperationTools = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.operationLinkButton = new System.Windows.Forms.RadioButton();
            this.operationButton = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.resetToHandCheckBox = new System.Windows.Forms.CheckBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.GroupBox();
            this.toXmlButton = new System.Windows.Forms.Button();
            this.convertToCodebutton = new System.Windows.Forms.Button();
            this.execButton = new System.Windows.Forms.Button();
            this.bandButton = new System.Windows.Forms.Button();
            this.moveButton = new System.Windows.Forms.Button();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.pictureHolderPanel = new System.Windows.Forms.Panel();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.checkVarsTimer = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog3 = new System.Windows.Forms.SaveFileDialog();
            this.commandsPanel.SuspendLayout();
            this.diagramCommandsPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageStateTools.SuspendLayout();
            this.tabPageComponentTools.SuspendLayout();
            this.tabPageOriginalOperationTools.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pictureHolderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.ImageIndex = 1;
            this.saveButton.ImageList = this.imageList1;
            this.saveButton.Location = new System.Drawing.Point(16, 56);
            this.saveButton.Name = "saveButton";
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "&Save";
            // 
            // imageList1
            // 
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // loadButton
            // 
            this.loadButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.loadButton.ImageIndex = 0;
            this.loadButton.ImageList = this.imageList1;
            this.loadButton.Location = new System.Drawing.Point(16, 24);
            this.loadButton.Name = "loadButton";
            this.loadButton.TabIndex = 0;
            this.loadButton.Text = "&Load";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "sm1";
            this.openFileDialog1.Filter = "State Machine Files|*.sm1";
            this.openFileDialog1.Title = "Select File to Load State Machine From";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "sm1";
            this.saveFileDialog1.Filter = "State Machine Files|*.sm1";
            this.saveFileDialog1.Title = "Select File to Save State Machine To";
            // 
            // newButton
            // 
            this.newButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.newButton.ImageIndex = 2;
            this.newButton.ImageList = this.imageList1;
            this.newButton.Location = new System.Drawing.Point(104, 24);
            this.newButton.Name = "newButton";
            this.newButton.TabIndex = 4;
            this.newButton.Text = "&New";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // drawingPanel
            // 
            this.drawingPanel.ContextMenu = this.contextMenu1;
            this.drawingPanel.Location = new System.Drawing.Point(0, 0);
            this.drawingPanel.Name = "drawingPanel";
            this.drawingPanel.Size = new System.Drawing.Size(2048, 2048);
            this.drawingPanel.TabIndex = 6;
            this.drawingPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TestStateGlyphForm_MouseUp);
            this.drawingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.drawingPanel_Paint);
            this.drawingPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.drawingPanel_KeyUp);
            this.drawingPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TestStateGlyphForm_MouseMove);
            this.drawingPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TestStateGlyphForm_MouseDown);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                         this.menuItem4,
                                                                                         this.menuItem5,
                                                                                         this.menuItem8});
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.newModelMenuItem,
                                                                                      this.loadFileMenuItem,
                                                                                      this.saveFileMenuItem});
            this.menuItem4.Text = "&File";
            // 
            // newModelMenuItem
            // 
            this.newModelMenuItem.Index = 0;
            this.newModelMenuItem.Text = "&New";
            this.newModelMenuItem.Click += new System.EventHandler(this.newButton_Click);
            // 
            // loadFileMenuItem
            // 
            this.loadFileMenuItem.Index = 1;
            this.loadFileMenuItem.Text = "&Load";
            // 
            // saveFileMenuItem
            // 
            this.saveFileMenuItem.Index = 2;
            this.saveFileMenuItem.Text = "&Save";
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.convertToCodeMenuItem,
                                                                                      this.executeModelMenuItem,
                                                                                      this.convertToXmlMenuItem});
            this.menuItem5.Text = "&Model";
            // 
            // convertToCodeMenuItem
            // 
            this.convertToCodeMenuItem.Index = 0;
            this.convertToCodeMenuItem.Text = "Convert To C&ode";
            // 
            // executeModelMenuItem
            // 
            this.executeModelMenuItem.Index = 1;
            this.executeModelMenuItem.Text = "&Execute";
            this.executeModelMenuItem.Click += new System.EventHandler(this.execButton_Click);
            // 
            // convertToXmlMenuItem
            // 
            this.convertToXmlMenuItem.Index = 2;
            this.convertToXmlMenuItem.Text = "Convert To &Xml";
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 2;
            this.menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.bandSelectorMenuItem,
                                                                                      this.handSelectorMenuItem});
            this.menuItem8.Text = "&Selector";
            // 
            // bandSelectorMenuItem
            // 
            this.bandSelectorMenuItem.Index = 0;
            this.bandSelectorMenuItem.Text = "&Band";
            this.bandSelectorMenuItem.Click += new System.EventHandler(this.bandButton_Click);
            // 
            // handSelectorMenuItem
            // 
            this.handSelectorMenuItem.Index = 1;
            this.handSelectorMenuItem.Text = "&Hand";
            this.handSelectorMenuItem.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // commandsPanel
            // 
            this.commandsPanel.Controls.Add(this.diagramCommandsPanel);
            this.commandsPanel.Controls.Add(this.panel1);
            this.commandsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.commandsPanel.Location = new System.Drawing.Point(0, 0);
            this.commandsPanel.Name = "commandsPanel";
            this.commandsPanel.Size = new System.Drawing.Size(1000, 88);
            this.commandsPanel.TabIndex = 7;
            // 
            // diagramCommandsPanel
            // 
            this.diagramCommandsPanel.Controls.Add(this.tabControl1);
            this.diagramCommandsPanel.Controls.Add(this.panel3);
            this.diagramCommandsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diagramCommandsPanel.Location = new System.Drawing.Point(392, 0);
            this.diagramCommandsPanel.Name = "diagramCommandsPanel";
            this.diagramCommandsPanel.Size = new System.Drawing.Size(608, 88);
            this.diagramCommandsPanel.TabIndex = 14;
            this.diagramCommandsPanel.TabStop = false;
            this.diagramCommandsPanel.Text = " Model Elements ";
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPageStateTools);
            this.tabControl1.Controls.Add(this.tabPageComponentTools);
            this.tabControl1.Controls.Add(this.tabPageOriginalOperationTools);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(112, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(493, 69);
            this.tabControl1.TabIndex = 50;
            // 
            // tabPageStateTools
            // 
            this.tabPageStateTools.Controls.Add(this.toolBarStateTools);
            this.tabPageStateTools.Location = new System.Drawing.Point(4, 25);
            this.tabPageStateTools.Name = "tabPageStateTools";
            this.tabPageStateTools.Size = new System.Drawing.Size(485, 40);
            this.tabPageStateTools.TabIndex = 0;
            this.tabPageStateTools.Text = "State Tools";
            // 
            // toolBarStateTools
            // 
            this.toolBarStateTools.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBarStateTools.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                                 this.toolBarButtonStateTool,
                                                                                                 this.toolBarButtonTransitionTool,
                                                                                                 this.toolBarButtonPortTool});
            this.toolBarStateTools.ButtonSize = new System.Drawing.Size(24, 24);
            this.toolBarStateTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolBarStateTools.DropDownArrows = true;
            this.toolBarStateTools.ImageList = this.imageListTools;
            this.toolBarStateTools.Location = new System.Drawing.Point(0, 0);
            this.toolBarStateTools.Name = "toolBarStateTools";
            this.toolBarStateTools.ShowToolTips = true;
            this.toolBarStateTools.Size = new System.Drawing.Size(485, 44);
            this.toolBarStateTools.TabIndex = 0;
            this.toolBarStateTools.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarStateTools_ButtonClick);
            // 
            // toolBarButtonStateTool
            // 
            this.toolBarButtonStateTool.ImageIndex = 0;
            this.toolBarButtonStateTool.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonStateTool.Tag = "CreateState";
            this.toolBarButtonStateTool.ToolTipText = "Draw States";
            // 
            // toolBarButtonTransitionTool
            // 
            this.toolBarButtonTransitionTool.ImageIndex = 1;
            this.toolBarButtonTransitionTool.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonTransitionTool.Tag = "CreateTransition";
            this.toolBarButtonTransitionTool.ToolTipText = "Draw Transitions";
            // 
            // toolBarButtonPortTool
            // 
            this.toolBarButtonPortTool.ImageIndex = 2;
            this.toolBarButtonPortTool.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonPortTool.Tag = "CreateStateTransitionPort";
            this.toolBarButtonPortTool.ToolTipText = "Draw Ports";
            // 
            // imageListTools
            // 
            this.imageListTools.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListTools.ImageSize = new System.Drawing.Size(32, 32);
            this.imageListTools.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTools.ImageStream")));
            this.imageListTools.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tabPageComponentTools
            // 
            this.tabPageComponentTools.Controls.Add(this.toolBarComponentTools);
            this.tabPageComponentTools.Location = new System.Drawing.Point(4, 25);
            this.tabPageComponentTools.Name = "tabPageComponentTools";
            this.tabPageComponentTools.Size = new System.Drawing.Size(485, 40);
            this.tabPageComponentTools.TabIndex = 1;
            this.tabPageComponentTools.Text = "Component Tools";
            this.tabPageComponentTools.Visible = false;
            // 
            // toolBarComponentTools
            // 
            this.toolBarComponentTools.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBarComponentTools.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                                     this.toolBarButtonComponentTool,
                                                                                                     this.toolBarButtonPortLinkTool});
            this.toolBarComponentTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolBarComponentTools.DropDownArrows = true;
            this.toolBarComponentTools.ImageList = this.imageListTools;
            this.toolBarComponentTools.Location = new System.Drawing.Point(0, 0);
            this.toolBarComponentTools.Name = "toolBarComponentTools";
            this.toolBarComponentTools.ShowToolTips = true;
            this.toolBarComponentTools.Size = new System.Drawing.Size(485, 42);
            this.toolBarComponentTools.TabIndex = 0;
            this.toolBarComponentTools.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarStateTools_ButtonClick);
            // 
            // toolBarButtonComponentTool
            // 
            this.toolBarButtonComponentTool.ImageIndex = 3;
            this.toolBarButtonComponentTool.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonComponentTool.Tag = "CreateComponent";
            this.toolBarButtonComponentTool.ToolTipText = "Draw Components";
            // 
            // toolBarButtonPortLinkTool
            // 
            this.toolBarButtonPortLinkTool.ImageIndex = 4;
            this.toolBarButtonPortLinkTool.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.toolBarButtonPortLinkTool.Tag = "CreatePortLink";
            this.toolBarButtonPortLinkTool.ToolTipText = "Draw PortLinks";
            // 
            // tabPageOriginalOperationTools
            // 
            this.tabPageOriginalOperationTools.Controls.Add(this.panel2);
            this.tabPageOriginalOperationTools.Location = new System.Drawing.Point(4, 25);
            this.tabPageOriginalOperationTools.Name = "tabPageOriginalOperationTools";
            this.tabPageOriginalOperationTools.Size = new System.Drawing.Size(485, 40);
            this.tabPageOriginalOperationTools.TabIndex = 2;
            this.tabPageOriginalOperationTools.Text = "Operations Tools";
            this.tabPageOriginalOperationTools.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.operationLinkButton);
            this.panel2.Controls.Add(this.operationButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(485, 40);
            this.panel2.TabIndex = 0;
            // 
            // operationLinkButton
            // 
            this.operationLinkButton.Location = new System.Drawing.Point(112, 4);
            this.operationLinkButton.Name = "operationLinkButton";
            this.operationLinkButton.TabIndex = 51;
            this.operationLinkButton.Tag = "CreateOperationPortLink";
            this.operationLinkButton.Text = "Op Link";
            this.operationLinkButton.Click += new System.EventHandler(this.modelElementButton_Click);
            // 
            // operationButton
            // 
            this.operationButton.Location = new System.Drawing.Point(4, 4);
            this.operationButton.Name = "operationButton";
            this.operationButton.TabIndex = 50;
            this.operationButton.Tag = "CreateOperation";
            this.operationButton.Text = "Operation";
            this.operationButton.Click += new System.EventHandler(this.modelElementButton_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.resetToHandCheckBox);
            this.panel3.Controls.Add(this.deleteButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(3, 16);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(109, 69);
            this.panel3.TabIndex = 49;
            // 
            // resetToHandCheckBox
            // 
            this.resetToHandCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.resetToHandCheckBox.Location = new System.Drawing.Point(6, 32);
            this.resetToHandCheckBox.Name = "resetToHandCheckBox";
            this.resetToHandCheckBox.TabIndex = 47;
            this.resetToHandCheckBox.Text = "Place One Element Only";
            // 
            // deleteButton
            // 
            this.deleteButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteButton.ImageIndex = 6;
            this.deleteButton.ImageList = this.imageList1;
            this.deleteButton.Location = new System.Drawing.Point(6, 8);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.TabIndex = 46;
            this.deleteButton.Text = "&Delete";
            // 
            // panel1
            // 
            this.panel1.ContextMenu = this.contextMenu1;
            this.panel1.Controls.Add(this.toXmlButton);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.loadButton);
            this.panel1.Controls.Add(this.newButton);
            this.panel1.Controls.Add(this.convertToCodebutton);
            this.panel1.Controls.Add(this.execButton);
            this.panel1.Controls.Add(this.bandButton);
            this.panel1.Controls.Add(this.moveButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 88);
            this.panel1.TabIndex = 13;
            this.panel1.TabStop = false;
            this.panel1.Text = " Actions ";
            // 
            // toXmlButton
            // 
            this.toXmlButton.Location = new System.Drawing.Point(104, 56);
            this.toXmlButton.Name = "toXmlButton";
            this.toXmlButton.Size = new System.Drawing.Size(80, 24);
            this.toXmlButton.TabIndex = 47;
            this.toXmlButton.Text = "To &Xml";
            // 
            // convertToCodebutton
            // 
            this.convertToCodebutton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.convertToCodebutton.ImageIndex = 3;
            this.convertToCodebutton.ImageList = this.imageList1;
            this.convertToCodebutton.Location = new System.Drawing.Point(192, 24);
            this.convertToCodebutton.Name = "convertToCodebutton";
            this.convertToCodebutton.Size = new System.Drawing.Size(88, 23);
            this.convertToCodebutton.TabIndex = 6;
            this.convertToCodebutton.Text = "To C&ode";
            // 
            // execButton
            // 
            this.execButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.execButton.ImageIndex = 4;
            this.execButton.ImageList = this.imageList1;
            this.execButton.Location = new System.Drawing.Point(192, 56);
            this.execButton.Name = "execButton";
            this.execButton.Size = new System.Drawing.Size(88, 23);
            this.execButton.TabIndex = 9;
            this.execButton.Text = "&Exec";
            this.execButton.Click += new System.EventHandler(this.execButton_Click);
            // 
            // bandButton
            // 
            this.bandButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bandButton.ImageList = this.imageList1;
            this.bandButton.Location = new System.Drawing.Point(296, 24);
            this.bandButton.Name = "bandButton";
            this.bandButton.Size = new System.Drawing.Size(80, 23);
            this.bandButton.TabIndex = 45;
            this.bandButton.Text = "&Band";
            this.bandButton.Click += new System.EventHandler(this.bandButton_Click);
            // 
            // moveButton
            // 
            this.moveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.moveButton.ImageIndex = 7;
            this.moveButton.ImageList = this.imageList1;
            this.moveButton.Location = new System.Drawing.Point(296, 56);
            this.moveButton.Name = "moveButton";
            this.moveButton.Size = new System.Drawing.Size(80, 23);
            this.moveButton.TabIndex = 46;
            this.moveButton.Text = "&Arrow";
            this.moveButton.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // saveFileDialog2
            // 
            this.saveFileDialog2.DefaultExt = "cs";
            this.saveFileDialog2.Filter = "CSharp File|*.cs";
            // 
            // pictureHolderPanel
            // 
            this.pictureHolderPanel.Controls.Add(this.hScrollBar1);
            this.pictureHolderPanel.Controls.Add(this.vScrollBar1);
            this.pictureHolderPanel.Controls.Add(this.drawingPanel);
            this.pictureHolderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureHolderPanel.Location = new System.Drawing.Point(0, 88);
            this.pictureHolderPanel.Name = "pictureHolderPanel";
            this.pictureHolderPanel.Size = new System.Drawing.Size(1000, 374);
            this.pictureHolderPanel.TabIndex = 8;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar1.Location = new System.Drawing.Point(0, 357);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(984, 17);
            this.hScrollBar1.TabIndex = 8;
            this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar1.Location = new System.Drawing.Point(984, 0);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(16, 374);
            this.vScrollBar1.TabIndex = 7;
            this.vScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
            // 
            // checkVarsTimer
            // 
            this.checkVarsTimer.Enabled = true;
            this.checkVarsTimer.Interval = 1000;
            this.checkVarsTimer.Tick += new System.EventHandler(this.checkVarsTimer_Tick);
            // 
            // saveFileDialog3
            // 
            this.saveFileDialog3.DefaultExt = "xml";
            this.saveFileDialog3.Filter = "Xml|*.xml";
            // 
            // TestStateGlyphControl
            // 
            this.Controls.Add(this.pictureHolderPanel);
            this.Controls.Add(this.commandsPanel);
            this.Name = "TestStateGlyphControl";
            this.Size = new System.Drawing.Size(1000, 462);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TestStateGlyphControl_KeyUp);
            this.commandsPanel.ResumeLayout(false);
            this.diagramCommandsPanel.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageStateTools.ResumeLayout(false);
            this.tabPageComponentTools.ResumeLayout(false);
            this.tabPageOriginalOperationTools.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pictureHolderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		DiagramModel _Model = new DiagramModel ();
		public DiagramModel Model { get { return _Model; } }

		public void ReplaceModel (DiagramModel model)
		{
			_Model = model;
		}

		public void ClearModel ()
		{
            if (AppForm () != null)
            {
                AppForm ().ClearExecutionModelTabs ();
            }
			_InteractionHandler = null;
			_Model = new DiagramModel ();
			_LastFileName = null;
			SelectMoveInteractor ();
		}

		protected void DoGlyphChange (IGlyph glyph)
		{
			_GlyphPropertyWindow.Glyph = glyph;
		}

		public void SelectGlyph (IGlyph glyph)
		{
			glyph.Selected = true;

			DoGlyphChange (glyph);
		}

		public void ShowHeader ()
		{
			_GlyphPropertyWindow.SetObject (_Model.Header);
		}

		bool Readonly { get { return _Model.Header.ReadOnly; } set { _Model.Header.ReadOnly = value; } }

		IUIInteractionHandler _InteractionHandler;

		private void TestStateGlyphForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            e = FactorOutZoom (e);
			if (_InteractionHandler != null)
			{
				_InteractionHandler.MouseDown (sender, e);
			}
			Refresh ();
		}

		private void TestStateGlyphForm_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            e = FactorOutZoom (e);
            if (_InteractionHandler != null)
			{
				_InteractionHandler.MouseMove (sender, e);
			}
		}
		
		private void TestStateGlyphForm_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            e = FactorOutZoom (e);
            if (_InteractionHandler != null)
			{
				_InteractionHandler.MouseUp (sender, e);
			}
		}

		public void SaveFile ()
		{
			_SaveFile.Execute ();
		}

		string _LastFileName;
		public string LastFileName 
		{
			get { return _LastFileName; }
			set { _LastFileName = value; }
		}

		private void newButton_Click(object sender, System.EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show ("Are you sure you want to clear all?", "Confirmation of Clear Requested", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
			if (dialogResult == DialogResult.Yes)
			{
				ClearModel ();
				ShowHeader ();
				Refresh ();
			}
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			if (Readonly)
			{
				return;
			}

			_Model.DeleteSelectedGlyphs ();
			Refresh ();
		}

		public void PaintDrawingArea (PaintEventArgs args)
		{
			drawingPanel_Paint (drawingPanel, args);
		}

		protected void SaveAsImage ()
		{
			ICommand command = new SaveAsImageCommand (_Model, this, _LastFileName);
			command.Execute ();
		}

		private void ResizeDrawingSurfaceToCoverDrawing ()
		{
			Rectangle bounds = _Model.GetDiagramBounds ();
			
			drawingPanel.ClientSize = new Size (bounds.Right, bounds.Bottom);
		}

        float _ZoomAmount = 1.0f;

		protected void Draw (IGraphicsContext gc)
		{
			foreach (IGlyph glyph in _Model.Glyphs)
			{
				glyph.Draw (gc);
			}		
			if (_InteractionHandler != null)
			{
				_InteractionHandler.Draw (gc);
			}
			DrawHeading (gc);
		}

		protected void DrawHeading (IGraphicsContext gc)
		{
			string s = string.Format ("{0}.{1} ver. {2} @ {3}", this.Model.Header.NameSpace, this.Model.Header.Name, this.Model.Header.StateMachineVersion, DateTime.Now.ToString ("s"));
			using (Brush brush = new SolidBrush (Color.Black))
			{
				gc.DrawString (s, brush, 10, new Point (5, 5), false);
			}
		}

		private void drawingPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.Clear (SystemColors.Window);
            e.Graphics.ScaleTransform (_ZoomAmount, _ZoomAmount);
			IGraphicsContext gc = new Implementation.DefaultGraphicsContext (e.Graphics);
			Draw (gc);
		}

		string lastDirectory;
		protected string FindFile (string fileName)
		{
			string[] files = System.IO.Directory.GetFiles (lastDirectory, fileName);
			return files [0];
		}

		public void SetStateMachine (string parentModelFile, qf4net.ILQHsm hsm)
		{
			lastDirectory = System.IO.Path.GetDirectoryName (parentModelFile);
			string fileName = FindFile (hsm.ModelInformation.FileName);

			LoadFileCommand command = new LoadFileCommand (fileName, this);
			command.Execute ();

			QHsmExecutionControllerView view = new QHsmExecutionControllerView ();
			string typeName = string.Format ("{0}.{1}, TestGeneratedStateMachines", _Model.Header.NameSpace, _Model.Header.Name);
			view.Controller = new QHsmExecutionController (_Model);
			view.Controller.Refresh += new EventHandler(RefreshView);
			view.SetMachine (hsm);
			StateDiagramView dv = this.Parent as StateDiagramView;
			dv.SetExecutionWindow (view);
			view.Show ();
		}

		public TestAppForm AppForm ()
		{
			Control parent = Parent;
			TestAppForm appForm = null;
			while (parent != null)
			{
				appForm = parent as TestAppForm;
				if (appForm != null) 
				{
					break;
				}
				parent = parent.Parent;
			}
			return appForm;
		}

		private void execButton_Click(object sender, System.EventArgs e)
		{
			AppForm ().ClearExecutionModelTabs ();

			try
			{
				ICommand command = new ExecuteHsmCommand (_Model, this, _LastFileName);
				command.Execute ();
			} 
			catch (Exception ex)
			{
				Exception ex2 = new Exception ("Model represented by " + _LastFileName + " could not be loaded for execution", ex);
				ThreadExceptionDialog dialog = new ThreadExceptionDialog (ex2);
				dialog.ShowDialog ();
			}
		}

		public void RefreshView ()
		{
			Refresh ();
		}

		private void drawingPanel_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				deleteButton_Click (sender, e);
			}
			else if (e.KeyCode == Keys.Oemplus)
			{
                bool isAlt = (e.Modifiers & Keys.Alt) == Keys.Alt;
                if (isAlt) // scale
                {
                    _ZoomAmount += .1f;   
                    RefreshView ();
                } 
                else 
                {
                    drawingPanel.Scale (1.1f);
                    drawingPanel.Location = new Point (0, 0);
                    SetScrollBars ();
                }

            }
			else if (e.KeyCode == Keys.OemMinus)
			{
                bool isAlt = (e.Modifiers & Keys.Alt) == Keys.Alt;
                if (isAlt) // scale
                {
                    _ZoomAmount -= .1f;   
                    RefreshView ();
                } 
                else 
                {
                    drawingPanel.Scale (0.9f);
                    drawingPanel.Location = new Point (0, 0);
                    SetScrollBars ();
                }
            }
			else if (e.KeyCode == Keys.End)
			{
				ResizeDrawingSurfaceToCoverDrawing ();
			}
			else if (e.KeyCode == Keys.F12)
			{
				SaveAsImage ();
			}
			else if (IsControlKey (e, Keys.R))
			{
				Readonly = !Readonly;
				CheckReadOnly ();
			}
			else 
			{
				if (_InteractionHandler != null)
				{
					_InteractionHandler.KeyUp (sender, e);
				}
			}
		}

		protected bool IsControlKey (KeyEventArgs e, Keys key)
		{
			bool isControl = (e.Modifiers & Keys.Control) == Keys.Control;
			if (isControl && e.KeyCode == key)
			{
				return true;
			}
			return false;
		}

		protected void CheckReadOnly ()
		{
			diagramCommandsPanel.Enabled = !Readonly;
		}

		string lastTitle;
		protected void CheckTitle () 
		{
			string currentName = "";
			if (_Model != null)
			{
				currentName = _Model.Header.Name;
				if (currentName == "" && _LastFileName != "")
				{
					currentName = Path.GetFileName (_LastFileName);
				}
			}

			if (currentName == null || currentName == "")
			{
				currentName = "NoName";
			}

			if (currentName != lastTitle)
			{
				lastTitle = currentName;
				Form parentForm = ParentStateDiagramView.Parent as Form;
                if (parentForm != null)
                {
                    System.Diagnostics.Debug.Assert (parentForm != null);
                    if (parentForm.IsMdiChild)
                    {
                        ParentStateDiagramView.Parent.Text = lastTitle;
                    }
                }
			}
		}

		private void TestStateGlyphControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			drawingPanel_KeyUp (sender, e);
		}

		private void checkVarsTimer_Tick(object sender, System.EventArgs e)
		{
			CheckReadOnly ();
			CheckTitle ();
		}

		public StateDiagramView ParentStateDiagramView { get { return this.Parent as StateDiagramView ; } }

		public void RefreshView (object sender, EventArgs e)
		{
			Refresh ();
		}

        private void modelElementButton_Click(object sender, System.EventArgs e)
        {
            ButtonBase button = sender as ButtonBase;
            System.Diagnostics.Debug.Assert (button != null);
            System.Diagnostics.Debug.Assert (button.Tag is string, "Model Element buttons must have tag assigned to the Method Name of the Glyph Factory to use");
            string modelElementMethod = button.Tag as string;
            setModelElement (modelElementMethod);
        }
	    
		private void setModelElement(string modelElementMethod)
	    {
			UIGlyphCreater creator = new UIGlyphCreater (this, modelElementMethod);
			creator.GlyphCreated += new MurphyPA.H2D.TestApp.UIGlyphCreater.GlyphCreatedHandler(creator_GlyphCreated);
			_InteractionHandler = creator;
		    this.drawingPanel.Cursor = Cursors.Cross;
		}

		protected void UnselectElementRadioButtons ()
		{
			foreach (Control control in diagramCommandsPanel.Controls)
			{
				RadioButton button = control as RadioButton;
				if (button != null)
				{
					button.Checked = false;
				}
			}
		}

		protected void SelectMoveInteractor ()
		{
			_InteractionHandler = new UIGlyphMoveAndReparent (this);
			UnselectElementRadioButtons ();
            this.drawingPanel.Cursor = Cursors.Arrow;
        }

		private void bandButton_Click(object sender, System.EventArgs e)
		{
			_InteractionHandler = new UIGlyphGroupSelector (this);
			UnselectElementRadioButtons ();		
		}

		private void moveButton_Click(object sender, System.EventArgs e)
		{
            DeselectAllOtherToolbars ();
            SelectMoveInteractor ();
		}

		public void SetCommandsPanelVisible (bool isVisible)
		{
			commandsPanel.Visible = isVisible;
		}

		public void LoadFile (string fileName)
		{
			openFileDialog1.FileName = fileName;
			_LoadFile.Execute ();
		}

        public void LoadFileDirect (string fileName)
        {
            LoadFileCommand command = new LoadFileCommand (fileName, this);
            command.Execute ();
        }

		private void creator_GlyphCreated(IGlyph glyph)
		{
			if (resetToHandCheckBox.Checked)
			{
				SelectMoveInteractor ();
			}
		}

		protected void CheckDefaultArgs ()
		{
			foreach (string arg in Environment.GetCommandLineArgs ())
			{
				if ("-OnlyOne" == arg)
				{
					resetToHandCheckBox.Checked = true;
				}
			}
		}

        private void hScrollBar1_ValueChanged(object sender, System.EventArgs e)
        {
            drawingPanel.Location = new Point (-hScrollBar1.Value, -vScrollBar1.Value);
        }

        void SetScrollBars ()
        {
            hScrollBar1.Maximum = drawingPanel.Width;
            vScrollBar1.Maximum = drawingPanel.Height;
        }

        protected PointF FactorOutZoom(PointF pt)
        {
            float x = pt.X / _ZoomAmount;
            float y = pt.Y / _ZoomAmount;
            pt = new PointF (x, y);
            return pt;
        }

        private void DeselectAllOtherToolbars ()
        {
            ToggleButtons (toolBarStateTools, null);
            ToggleButtons (toolBarComponentTools, null);
        }

	    private void ToggleButtons(ToolBar toolBar, ToolBarButton ignoreButton)
	    {
	        foreach (ToolBarButton button in toolBar.Buttons)
	        {
	            if(button != ignoreButton)
	            {
	                button.Pushed = false;
	            }
	        }
	    }
	    
        private void toolBarStateTools_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            ToggleButtons ((ToolBar)sender, e.Button);            
            
            ToolBarButton button = e.Button;
            System.Diagnostics.Debug.Assert (button != null);
            System.Diagnostics.Debug.Assert (button.Tag is string, "Model Element buttons must have tag assigned to the Method Name of the Glyph Factory to use");
            string modelElementMethod = button.Tag as string;
            setModelElement (modelElementMethod);            
        }

	    protected MouseEventArgs FactorOutZoom(MouseEventArgs e)
	    {
            PointF pt = new PointF (e.X, e.Y);
            pt = FactorOutZoom (pt);
            Point rpt = new Point ((int)pt.X, (int)pt.Y);
            MouseEventArgs result = new MouseEventArgs (e.Button, e.Clicks, rpt.X, rpt.Y, e.Delta);
            return result;
	    }
	}
}
