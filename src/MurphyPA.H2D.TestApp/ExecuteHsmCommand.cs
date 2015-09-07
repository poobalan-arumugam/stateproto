using System;
using System.Collections;
using System.Windows.Forms;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ExecuteHsmCommand.
	/// </summary>
	public class ExecuteHsmCommand : ICommand
	{
		DiagramModel _Model;
		IUIInterationContext _Context;
		string _LastFileName;

		public ExecuteHsmCommand(DiagramModel model, IUIInterationContext context, string lastFileName)
		{
			_Model = model;
			_Context = context;
			_LastFileName = lastFileName;
		}

		#region ICommand Members

		public void Execute()
		{
			if (_Model.HasComponentFrameElements ())
			{
				ExecuteComponentFrame ();
			} 
			else if (_Model.HasStateElements ())
			{
				ExecuteHsm ();
			}
		}

		protected void ExecuteComponentFrame ()
		{
			ProcessComponentFrame frame = new ProcessComponentFrame (_Model, _Context.AppForm ());
			frame.Refresh += new EventHandler(_Context.RefreshView);
			ProcessComponentFrameExecutionView view = new ProcessComponentFrameExecutionView ();
			view.Init (frame);
			StateDiagramView dv = _Context.ParentStateDiagramView;
			dv.SetExecutionWindow (view);
			view.Show ();

			foreach (DictionaryEntry de in frame.ComponentContexts)
			{
				ProcessComponentFrame.ComponentContext ctx = de.Value as ProcessComponentFrame.ComponentContext;
				ExecuteHsm (ctx.ComponentName, ctx.Hsm);
			}
		}

		protected void ExecuteHsm ()
		{
			string typeName = string.Format ("{0}.{1}, {2}", _Model.Header.NameSpace, _Model.Header.Name, _Model.Header.Assembly);
			ExecuteHsm (typeName);
		}

		protected void ExecuteHsm (string modelName, qf4net.ILQHsm hsm)
		{
			TestAppForm appForm = _Context.AppForm ();
			try 
			{
				qf4net.ModelInformation modelInformation = hsm.ModelInformation;

				StateDiagramView sd = new StateDiagramView (false);
				sd.StateControl.SetStateMachine (_LastFileName, hsm);

#warning Cleanup this code - this control uses knowledge of its parent - below (TestAppForm) and above (StateDiagramView)
				// find Top level form

				if (appForm != null)
				{
					appForm.AddChild (modelName, modelName, sd);
					appForm.RegisterHsm (hsm);
				}
				else 
				{
					Form frm = new Form ();
					frm.Text = modelName;
					frm.Controls.Add (sd);
					sd.Dock = DockStyle.Fill;
					frm.Show ();
				}
			} 
			catch {}
		}

		protected void ExecuteHsm (string typeName)
		{
			/*
			ExecutionControllerView view = new ExecutionControllerView ();
			view.Controller = new ExecutionController (_Glyphs);
			view.Controller.Refresh += new EventHandler(Controller_Refresh);
			view.Show ();
			*/
			QHsmExecutionControllerView view = new QHsmExecutionControllerView ();
			view.Controller = new QHsmExecutionController (_Model);
			view.Controller.Refresh += new EventHandler(_Context.RefreshView);
			view.SetMachineName (typeName);
			//view.SetMachineModel (_Model, AppForm ());
#warning Cleanup this code - this control uses knowledge of its parent 
			StateDiagramView dv = _Context.ParentStateDiagramView;
			dv.SetExecutionWindow (view);
			view.Show ();


			qf4net.ILQHsm hsm = view.Controller.Hsm;

			TestAppForm appForm = _Context.AppForm ();
			if (appForm != null)
			{
				appForm.RegisterHsm (hsm);
			}

			qf4net.IQSupportsSubMachines supportsSubMachines = hsm as qf4net.IQSupportsSubMachines;
			if (supportsSubMachines != null)
			{
				foreach (DictionaryEntry de in supportsSubMachines.SubMachines)
				{
					qf4net.ILQHsm subMachine = de.Value as qf4net.ILQHsm;
					ExecuteHsm (de.Key.ToString (), subMachine);
				}
			}
		}
		#endregion
	}
}
