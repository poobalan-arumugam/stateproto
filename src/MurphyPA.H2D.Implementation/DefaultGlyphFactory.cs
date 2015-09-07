using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for DefaultGlyphFactory.
	/// </summary>
	public class DefaultGlyphFactory  : IGlyphFactory
	{
		#region IGlyphFactory Members - Default Constructors

		public IStateGlyph CreateState()
		{
			return new StateGlyph ();
		}

		public ITransitionGlyph CreateTransition()
		{
			return new TransitionGlyph ();
		}

		public IStateTransitionPortGlyph CreateStateTransitionPort ()
		{
			return new StateTransitionPortGlyph ();
		}

		public IComponentGlyph CreateComponent()
		{
			return new ComponentGlyph ();
		}

		public IPortLinkGlyph CreatePortLink()
		{
			return new PortLinkGlyph ();
		}

		public IOperationPortLinkGlyph CreateOperationPortLink()
		{
			return new OperationPortLinkGlyph ();
		}

		public IOperationGlyph CreateOperation()
		{
			return new OperationGlyph ();
		}

		#endregion

		#region IGlyphFactory Members - Id, Bounds

		public IPortLinkGlyph CreatePortLink(string id, System.Drawing.Rectangle bounds)
		{
			return new PortLinkGlyph (id, bounds);
		}

		public IOperationPortLinkGlyph CreateOperationPortLink(string id, System.Drawing.Rectangle bounds)
		{
			return new OperationPortLinkGlyph (id, bounds);
		}

		public IStateGlyph CreateState(string id, System.Drawing.Rectangle bounds)
		{
			return new StateGlyph (id, bounds);
		}

		public ITransitionGlyph CreateTransition(string id, System.Drawing.Rectangle bounds)
		{
			return new TransitionGlyph (id, bounds);
		}

		public IComponentGlyph CreateComponent(string id, System.Drawing.Rectangle bounds)
		{
			return new ComponentGlyph (id, bounds);
		}

		public IStateTransitionPortGlyph CreateStateTransitionPort(string id, System.Drawing.Rectangle bounds)
		{
			return new StateTransitionPortGlyph (id, bounds);
		}

		public IOperationGlyph CreateOperation(string id, System.Drawing.Rectangle bounds)
		{
			return new OperationGlyph (id, bounds);
		}

		#endregion

		#region IGlyphFactory Members - id, point

		Rectangle GetBoundsFrom (Point point, IGlyph glyph)
		{
			Rectangle bounds = glyph.Bounds;
			return new Rectangle (point, bounds.Size);
		}

		public IPortLinkGlyph CreatePortLink(string id, Point point)
		{
			return CreatePortLink (id, GetBoundsFrom (point, CreatePortLink ()));
		}

		public IOperationPortLinkGlyph CreateOperationPortLink(string id, Point point)
		{
			return CreateOperationPortLink (id, GetBoundsFrom (point, CreateOperationPortLink ()));
		}

		public IStateGlyph CreateState(string id, Point point)
		{
			return new StateGlyph (id, point);
		}

		public ITransitionGlyph CreateTransition(string id, Point point)
		{
			return new TransitionGlyph (id, point);
		}

		public IComponentGlyph CreateComponent(string id, Point point)
		{
			return CreateComponent (id, GetBoundsFrom (point, CreateComponent ()));
		}

		public IStateTransitionPortGlyph CreateStateTransitionPort(string id, Point point)
		{
			return CreateStateTransitionPort (id, GetBoundsFrom (point, CreateStateTransitionPort ()));
		}

		public IOperationGlyph CreateOperation(string id, Point point)
		{
			return CreateOperation (id, GetBoundsFrom (point, CreateOperation ()));
		}

		#endregion
	}
}
