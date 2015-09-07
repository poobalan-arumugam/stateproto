using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IGlyphVisitor.
	/// </summary>
	public interface IGlyphVisitor
	{
		void Visit (IGlyph glyph);
		void Visit (ICompositeGlyph composite);
		void Visit (IGroupGlyph group);
		void Visit (IStateGlyph state);
		void Visit (ITransitionGlyph transition);
		void Visit (ITransitionContactPointGlyph transitionContactPoint);
		void Visit (IPortLinkContactPointGlyph portLinkContactPoint);
		void Visit (IPortLinkGlyph portLink);
		void Visit (IComponentGlyph component);
		void Visit (IStateTransitionPortGlyph port);
	}
}
