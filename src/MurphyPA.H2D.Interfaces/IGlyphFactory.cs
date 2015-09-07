using System;
using System.Drawing;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IGlyphFactory.
	/// </summary>
	public interface IGlyphFactory
	{
		IStateGlyph CreateState ();
		[DirectionalGlyph] ITransitionGlyph CreateTransition ();
		IStateTransitionPortGlyph CreateStateTransitionPort ();
		IComponentGlyph CreateComponent ();
		[DirectionalGlyph] IPortLinkGlyph CreatePortLink ();
		IOperationGlyph CreateOperation ();
		[DirectionalGlyph] IOperationPortLinkGlyph CreateOperationPortLink ();

		IStateGlyph CreateState (string id, Point point);
		[DirectionalGlyph] ITransitionGlyph CreateTransition (string id, Point point);
		IStateTransitionPortGlyph CreateStateTransitionPort (string id, Point point);
		IComponentGlyph CreateComponent (string id, Point point);
		[DirectionalGlyph] IPortLinkGlyph CreatePortLink (string id, Point point);
		IOperationGlyph CreateOperation (string id, Point point);
		[DirectionalGlyph] IOperationPortLinkGlyph CreateOperationPortLink (string id, Point point);

		IStateGlyph CreateState (string id, Rectangle bounds);
		[DirectionalGlyph] ITransitionGlyph CreateTransition (string id, Rectangle bounds);
		IStateTransitionPortGlyph CreateStateTransitionPort (string id, Rectangle bounds);
		IComponentGlyph CreateComponent (string id, Rectangle bounds);
		[DirectionalGlyph] IPortLinkGlyph CreatePortLink (string id, Rectangle bounds);
		IOperationGlyph CreateOperation (string id, Rectangle bounds);
		[DirectionalGlyph] IOperationPortLinkGlyph CreateOperationPortLink (string id, Rectangle bounds);
	}
}
