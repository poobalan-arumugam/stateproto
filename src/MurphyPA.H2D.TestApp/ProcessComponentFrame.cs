using System;
using System.Collections;
using MurphyPA.H2D.Interfaces;
using qf4net;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ProcessComponentFrame.
	/// </summary>
	public class ProcessComponentFrame
	{
		DiagramModel _Model;
		ArrayList _Components;
		ArrayList _Ports;

		TestAppForm _AppForm;

		public ProcessComponentFrame (DiagramModel model, TestAppForm appForm)
		{
			_AppForm = appForm;
			_Model = model;
			Prepare ();
			Process ();
		}

		protected void Prepare ()
		{
			_Components = new ArrayList ();
			_Ports = new ArrayList ();
			foreach (IGlyph glyph in _Model.Glyphs)
			{
				if (glyph is IComponentGlyph) 
				{
					_Components.Add (glyph);
				}
				else if (glyph is IPortLinkGlyph)
				{
					_Ports.Add (glyph);
				}
			}

			_Ports.Sort (new PortComparer ());
		}

		protected class PortComparer : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				if (x == y) 
				{
					return 0;
				}

				IPortLinkGlyph xPort = x as IPortLinkGlyph;
				IPortLinkGlyph yPort = y as IPortLinkGlyph;

				int fromPortComp = xPort.FromPortName.CompareTo (yPort.FromPortName);
				if (fromPortComp == 0)
				{
					return xPort.SendIndex.CompareTo (yPort.SendIndex);
				}
				return fromPortComp;
			}

			#endregion
		}


		protected IComponentGlyph GetComponent (IPortLinkGlyph portLink, IPortLinkContactPointGlyph contactPoint)
		{
			IComponentGlyph parent = contactPoint.Parent as IComponentGlyph;
			return parent;
		}

		protected IComponentGlyph GetComponent (IPortLinkGlyph portLink, TransitionContactEnd whichEnd)
		{
			foreach (IPortLinkContactPointGlyph contactPoint in portLink.ContactPoints)
			{
				if (contactPoint.WhichEnd == whichEnd)
				{
					return GetComponent (portLink, contactPoint);
				}
			}
			return null;
		}

		protected void Log (System.Drawing.Color color, string fmt, params object[] args)
		{
			string msg = fmt;
			if (args != null && args.Length > 0)
			{
				msg = string.Format (fmt, args);
			}
			_AppForm.Log (color, msg + "\n");
		}


		public class ComponentContext 
		{
			ILQHsm _Hsm;
			public ILQHsm Hsm { get { return _Hsm; } set { _Hsm = value; } }

			public string ComponentName { get { return _Component.Name; } }

			IComponentGlyph _Component;

			public ComponentContext (object hsm, IComponentGlyph comp)
			{
				_Component = comp;

				_Hsm = hsm as ILQHsm; 
				if (_Hsm == null)
				{
					throw new NullReferenceException ("Hsm created using " + _Component.TypeName + " is null");
				}
			}
		}

		IQPort GetPort (ComponentContext ctx, string portName)
		{
			Type type = ctx.Hsm.GetType ();
			System.Reflection.PropertyInfo propInfo = type.GetProperty (portName);
			if (propInfo == null)
			{
				throw new NullReferenceException ("Port [" + portName + "] not found on Component " + ctx.ComponentName + " - " + ctx.Hsm);
			}
			object port = propInfo.GetValue (ctx.Hsm, null);
			IQPort qport = port as IQPort;
			return qport;
		}

		protected void LinkPorts (ComponentContext ctxFrom, ComponentContext ctxTo, IPortLinkGlyph portLink)
		{
			string fromPortName = portLink.FromPortName;
			string toPortName = portLink.ToPortName;

			IQPort portFrom = GetPort (ctxFrom, fromPortName);
			IQPort toPort = GetPort (ctxTo, toPortName);

			portFrom.QEvents += new QEventHandler(toPort.Receive);

			AddPortLinkToPortContext (portFrom, portLink);
		}

		protected void AddPortLinkToPortContext (IQPort port, IPortLinkGlyph portLink)
		{
			if (!_PortContext.Contains (port))
			{
				_PortContext.Add (port, new ArrayList ());
			}

			ArrayList list = _PortContext [port] as ArrayList; 
			list.Add (portLink);
		}

		Hashtable _ComponentContexts;
		public Hashtable ComponentContexts 
		{
			get { return _ComponentContexts; }
		}

		Hashtable _PortContext = new Hashtable ();

		protected void Process ()
		{
			_ComponentContexts = new Hashtable ();
			foreach (IComponentGlyph component in _Components)
			{
				Log (System.Drawing.Color.Green, "{1} {0} = new {1} ();", component.Name, component.TypeName);
				Type type = Type.GetType (component.TypeName);
				if (type == null)
				{
					throw new NullReferenceException ("Type [" + component.TypeName + "] not found");
				}
				object hsm = HsmUtil.CreateHsm (type);
				_ComponentContexts.Add (component.Name, new ComponentContext (hsm, component));
			}
			foreach (IPortLinkGlyph portLink in _Ports)
			{
				IComponentGlyph compFrom = GetComponent (portLink, TransitionContactEnd.From);
				IComponentGlyph compTo = GetComponent (portLink, TransitionContactEnd.To);
				
				Log (System.Drawing.Color.Blue, "{0}.{1}.QEvents += new QEventHandler ({2}.{3}.Receive);", compFrom.Name, portLink.FromPortName, compTo.Name, portLink.ToPortName);

				ComponentContext compCtxFrom = _ComponentContexts [compFrom.Name] as ComponentContext;
				ComponentContext compCtxTo = _ComponentContexts [compTo.Name] as ComponentContext;
				LinkPorts (compCtxFrom, compCtxTo, portLink);
			}

			foreach (DictionaryEntry de in _PortContext)
			{
				IQPort port = de.Key as IQPort;
				port.QEvents += new QEventHandler(port_QEvents);
			}
		}

		private void port_QEvents(IQPort port, IQEvent ev)
		{
			foreach (IPortLinkGlyph portLink in _Ports)
			{
				portLink.Selected = false;
			}

			if (_PortContext.Contains (port))
			{
				ArrayList list = _PortContext [port] as ArrayList; 
				foreach (IPortLinkGlyph portLink in list)
				{
					_AppForm.Log (System.Drawing.Color.DeepSkyBlue, "Port: " + portLink.ToPortName + "->" + portLink.FromPortName + " ev: " + ev + "\n");
					portLink.Selected = true;
				}
			}
			DoRefresh ();
		}

		public event EventHandler Refresh;

		void DoRefresh ()
		{
			if (Refresh != null)
			{
				Refresh (this, new EventArgs ());
			}
		}
	}
}
