using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ExecutionControllerView.
	/// </summary>
	public class ExecutionControllerView : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button stepButton;
		private System.Windows.Forms.ListBox transitionListView;
		private System.Windows.Forms.Button injectButton;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.ListBox eventQueueListView;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ExecutionControllerView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.startButton = new System.Windows.Forms.Button();
			this.stepButton = new System.Windows.Forms.Button();
			this.transitionListView = new System.Windows.Forms.ListBox();
			this.injectButton = new System.Windows.Forms.Button();
			this.stopButton = new System.Windows.Forms.Button();
			this.eventQueueListView = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(8, 8);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 0;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// stepButton
			// 
			this.stepButton.Location = new System.Drawing.Point(8, 40);
			this.stepButton.Name = "stepButton";
			this.stepButton.TabIndex = 1;
			this.stepButton.Text = "S&tep";
			this.stepButton.Click += new System.EventHandler(this.stepButton_Click);
			// 
			// transitionListView
			// 
			this.transitionListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.transitionListView.Location = new System.Drawing.Point(144, 32);
			this.transitionListView.Name = "transitionListView";
			this.transitionListView.Size = new System.Drawing.Size(432, 277);
			this.transitionListView.TabIndex = 2;
			// 
			// injectButton
			// 
			this.injectButton.Location = new System.Drawing.Point(144, 328);
			this.injectButton.Name = "injectButton";
			this.injectButton.Size = new System.Drawing.Size(96, 23);
			this.injectButton.TabIndex = 3;
			this.injectButton.Text = "&Inject Event";
			this.injectButton.Click += new System.EventHandler(this.injectButton_Click);
			// 
			// stopButton
			// 
			this.stopButton.Location = new System.Drawing.Point(8, 136);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 4;
			this.stopButton.Text = "St&op";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// eventQueueListView
			// 
			this.eventQueueListView.Location = new System.Drawing.Point(144, 368);
			this.eventQueueListView.Name = "eventQueueListView";
			this.eventQueueListView.Size = new System.Drawing.Size(432, 82);
			this.eventQueueListView.TabIndex = 5;
			// 
			// ExecutionControllerView
			// 
			this.ClientSize = new System.Drawing.Size(600, 470);
			this.Controls.Add(this.eventQueueListView);
			this.Controls.Add(this.stopButton);
			this.Controls.Add(this.injectButton);
			this.Controls.Add(this.transitionListView);
			this.Controls.Add(this.stepButton);
			this.Controls.Add(this.startButton);
			this.Name = "ExecutionControllerView";
			this.Text = "ExecutionControllerView";
			this.Load += new System.EventHandler(this.ExecutionControllerView_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void ExecutionControllerView_Load(object sender, System.EventArgs e)
		{
		
		}

		ExecutionController _Controller;

		private void startButton_Click(object sender, System.EventArgs e)
		{
			_Controller.Start ();
		}

		private void stepButton_Click(object sender, System.EventArgs e)
		{
			_Controller.Step ();
		}
	
		public ExecutionController Controller 
		{
			get { return _Controller; } 
			set 
			{ 
				if (_Controller != null)
				{
					_Controller.NewTransitionList -= new EventHandler(_Controller_NewTransitionList);
				}
				_Controller = value; 
				if (_Controller != null)
				{
					_Controller.NewTransitionList += new EventHandler(_Controller_NewTransitionList);
					_Controller.NoEvents += new EventHandler(_Controller_NoEvents);
					_Controller.TransitionEvent += new EventHandler(_Controller_TransitionEvent);
					_Controller.DropEvent += new EventHandler(_Controller_DropEvent);
				}
			}
		}

		protected class TransitionInfoHolder 
		{
			ExecutionController.TransitionInfo _TransitionInfo;
			public ExecutionController.TransitionInfo TransitionInfo { get { return _TransitionInfo; } }

			public TransitionInfoHolder (ExecutionController.TransitionInfo transitionInfo)
			{
				_TransitionInfo = transitionInfo;
			}

			public override string ToString()
			{
				return _TransitionInfo.Transition.DisplayText ();
			}

		}

		private void _Controller_NewTransitionList(object sender, EventArgs e)
		{
			transitionListView.BeginUpdate ();
			try 
			{
				transitionListView.Items.Clear ();

				ExecutionController.TransitionListEventArgs ea = e as ExecutionController.TransitionListEventArgs;
				foreach (ExecutionController.TransitionInfo info in ea.TransitionList)
				{
					transitionListView.Items.Add (new TransitionInfoHolder (info));
				}
			} 
			finally 
			{
				transitionListView.EndUpdate ();
			}
		}

		private void injectButton_Click(object sender, System.EventArgs e)
		{
			if (transitionListView.SelectedItem != null)
			{
				TransitionInfoHolder holder = transitionListView.SelectedItem as TransitionInfoHolder;
				if (holder != null)
				{
					_Controller.InjectEvent (holder.TransitionInfo.Transition.Event);
					RefreshEventQueueView ();
				}
			}
		}

		void RefreshEventQueueView ()
		{
			ArrayList list = new ArrayList (_Controller.EventQueue);
			eventQueueListView.DataSource = list;
		}

		private void stopButton_Click(object sender, System.EventArgs e)
		{
			_Controller.Stop ();
		}

		private void _Controller_NoEvents(object sender, EventArgs e)
		{

		}

		private void _Controller_TransitionEvent(object sender, EventArgs e)
		{
			RefreshEventQueueView ();
		}

		private void _Controller_DropEvent(object sender, EventArgs e)
		{
			RefreshEventQueueView ();
		}
	}
}
