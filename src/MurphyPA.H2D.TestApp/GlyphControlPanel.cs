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
	/// GlyphControlPanel.
	/// </summary>
	public class GlyphControlPanel : System.Windows.Forms.UserControl, IUIInterationContext
	{
		private DrawingSurface drawingPanel;
		private System.Windows.Forms.Panel pictureHolderPanel;
        private System.ComponentModel.Container components = null;

		public GlyphControlPanel ()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

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
            this.drawingPanel = new MurphyPA.H2D.TestApp.DrawingSurface();
            this.pictureHolderPanel = new System.Windows.Forms.Panel();
            this.pictureHolderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // drawingPanel
            // 
            this.drawingPanel.Location = new System.Drawing.Point(0, 0);
            this.drawingPanel.Name = "drawingPanel";
            this.drawingPanel.Size = new System.Drawing.Size(2048, 2048);
            this.drawingPanel.TabIndex = 6;
            this.drawingPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.drawingPanel_MouseUp);
            this.drawingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.drawingPanel_Paint);
            this.drawingPanel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.drawingPanel_KeyUp);
            this.drawingPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.drawingPanel_MouseMove);
            this.drawingPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.drawingPanel_MouseDown);
            // 
            // pictureHolderPanel
            // 
            this.pictureHolderPanel.AutoScroll = true;
            this.pictureHolderPanel.Controls.Add(this.drawingPanel);
            this.pictureHolderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureHolderPanel.Location = new System.Drawing.Point(0, 0);
            this.pictureHolderPanel.Name = "pictureHolderPanel";
            this.pictureHolderPanel.Size = new System.Drawing.Size(1000, 462);
            this.pictureHolderPanel.TabIndex = 8;
            // 
            // GlyphControlPanel
            // 
            this.Controls.Add(this.pictureHolderPanel);
            this.Name = "GlyphControlPanel";
            this.Size = new System.Drawing.Size(1000, 462);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.drawingPanel_KeyUp);
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

        #region event ModelCleared
        protected void ClearModel_i ()
        {
            DoModelCleared ();

            _InteractionHandler = null;
            _Model = new DiagramModel ();
            _LastFileName = null;
            SelectMoveInteractor ();
        }

        public event EventHandler ModelCleared;

        protected void RaiseModelCleared (EventHandler handler)
        {
            if (handler != null)
            {
                handler (this, new EventArgs ());
            }
        }

        protected void DoModelCleared ()
        {
            RaiseModelCleared (ModelCleared);
        }
        #endregion

        #region event GlyphChange
        public event GlyphChangeHandler GlyphCreated;

        protected void RaiseGlyphCreated (GlyphChangeHandler handler, object sender, IGlyph glyph)
        {
            if (handler != null)
            {
                handler (sender, glyph);
            }
        }

        protected void DoGlyphCreated (IGlyph glyph)
        {
            RaiseGlyphCreated (GlyphCreated, this, glyph);
        }
        #endregion

        #region event GlyphChange
        public event GlyphChangeHandler GlyphChange;

        protected void RaiseGlyphChange (GlyphChangeHandler handler, object sender, IGlyph glyph)
        {
            if (handler != null)
            {
                handler (sender, glyph);
            }
        }

		protected void DoGlyphChange (IGlyph glyph)
		{
            RaiseGlyphChange (GlyphChange, this, glyph);
		}
        #endregion

        #region event ReadOnlyChange
        public event ReadOnlyChangeHandler ReadOnlyChange;

        protected void RaiseReadOnlyChange (ReadOnlyChangeHandler handler, IUIInterationContext context)
        {
            if (handler != null)
            {
                handler (context);
            }
        }

        protected void DoReadOnlyChange ()
        {
            RaiseReadOnlyChange (ReadOnlyChange, this);
        }
        #endregion

        #region event InteractionHandlerChange
        public event InteractionHandlerChangeHandler InteractionHandlerChange;

        protected void RaiseInteractionHandlerChange (InteractionHandlerChangeHandler handler, IUIInteractionHandler interactionHandler)
        {
            if (handler != null)
            {
                handler (this, interactionHandler);
            }
        }

        protected void DoInteractionHandlerChange (IUIInteractionHandler interactionHandler)
        {
            RaiseInteractionHandlerChange (InteractionHandlerChange, interactionHandler);
        }
        #endregion

        #region event Log
        public event LogHandler Log;

        protected void DoLog (Color color, string fmt, params object[] args)
        {
            LogHandler handler = Log;
            if (handler != null)
            {
                handler (color, fmt, args);
            }
        }

        public void Info (Color color, string fmt, params object[] args)
        {
            DoLog (color, fmt, args);   
        }
        #endregion

		public void SelectGlyph (IGlyph glyph)
		{
			glyph.Selected = true;

			DoGlyphChange (glyph);
		}

		public void ShowHeader ()
		{
			//_GlyphPropertyWindow.SetObject (_Model.Header);
		}

		public bool Readonly { get { return _Model.Header.ReadOnly; } set { _Model.Header.ReadOnly = value; } }

		IUIInteractionHandler _InteractionHandler;

		private void drawingPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_InteractionHandler != null)
			{
				_InteractionHandler.MouseDown (sender, e);
			}
			Refresh ();
		}

		private void drawingPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_InteractionHandler != null)
			{
				_InteractionHandler.MouseMove (sender, e);
			}
		}
		
		private void drawingPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_InteractionHandler != null)
			{
				_InteractionHandler.MouseUp (sender, e);
			}
		}

		string _LastFileName;
		public string LastFileName 
		{
			get { return _LastFileName; }
			set { _LastFileName = value; }
		}

        public void ClearModel ()
        {
            ClearModel_i ();
            ShowHeader ();
            Refresh ();
        }

		protected void DeleteSelectedGlyphs ()
		{
			if (Readonly)
			{
				return;
			}

			_Model.DeleteSelectedGlyphs ();
			RefreshView ();
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

		protected void ResizeDrawingSurfaceToCoverDrawing ()
		{
			Rectangle bounds = _Model.GetDiagramBounds ();
			
			drawingPanel.ClientSize = new Size (bounds.Right, bounds.Bottom);
		}

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
			IGraphicsContext gc = new Implementation.DefaultGraphicsContext (e.Graphics);
			Draw (gc);
		}

		public void RefreshView ()
		{
			Refresh ();
		}

		protected virtual void drawingPanel_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Oemplus)
			{
				drawingPanel.Scale (1.1f);
				drawingPanel.Location = new Point (0, 0);
			}
			else if (e.KeyCode == Keys.OemMinus)
			{
				drawingPanel.Scale (0.9f);
				drawingPanel.Location = new Point (0, 0);
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
				DoReadOnlyChange ();
			}
			else 
			{
                if (!Readonly)
                {
                    if (e.KeyCode == Keys.Delete)
                    {
                        DeleteSelectedGlyphs ();
                    }
                    else if (_InteractionHandler != null)
                    {
                        _InteractionHandler.KeyUp (sender, e);
                    }
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

        public void SelectGlyphCreatorInteractor (string modelElementMethod)
        {
            UIGlyphCreater creator = new UIGlyphCreater (this, modelElementMethod);
            creator.GlyphCreated += new MurphyPA.H2D.TestApp.UIGlyphCreater.GlyphCreatedHandler(creator_GlyphCreated);
            _InteractionHandler = creator;
        }

        private void creator_GlyphCreated(IGlyph glyph)
        {
            DoGlyphCreated (glyph);
        }

		public void SelectMoveInteractor ()
		{
			_InteractionHandler = new UIGlyphMoveAndReparent (this);
			DoInteractionHandlerChange (_InteractionHandler);
		}

		public void SelectBandInteractor ()
		{
			_InteractionHandler = new UIGlyphGroupSelector (this);
			DoInteractionHandlerChange (_InteractionHandler);		
		}

		public void LoadFile (string fileName)
		{
            LoadFileCommand command = new LoadFileCommand (fileName, this);
            command.Execute ();
		}


        public void RegisterHsm (qf4net.ILQHsm hsm) { throw new NotImplementedException (); }
        public void SetExecutionWindow (Control control) { throw new NotImplementedException (); }
        public void AddChild (string id, string name, Control control) { throw new NotImplementedException (); }

        public IWin32Window ParentWindow 
        {
            get
            {
                return this.TopLevelControl as IWin32Window;  
            } 
        }

	}
}
