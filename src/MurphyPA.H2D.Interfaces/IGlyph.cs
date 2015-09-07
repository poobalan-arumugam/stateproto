using System;
using System.Drawing;
using System.Collections;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IGlyph.
	/// </summary>
	public interface IGlyph
	{
		string Id { get; }
		string Name { get; set; }
		string FullyQualifiedStateName { get; }
		string Note { get; set; }

		bool ContainsPoint (Point point);
		void Draw (IGraphicsContext gc);

		void MoveTo (Point point);
		void Offset (Point point);

		bool Selected { get; set; }

		IGlyph Parent { get; set; }
		IEnumerable Children { get; }

		IGlyph Owner { get; set; }
		IEnumerable OwnedItems { get; }
 
		void AddChild (IGlyph child);
		void RemoveChild (IGlyph child);

		void AddOwned (IGlyph owned);
		void RemoveOwned (IGlyph owned);

		Rectangle Bounds { get; }

		event OffsetChangedHandler OffsetChanged;

		void Accept (IGlyphVisitor visitor);

		bool DoNotInstrument { get; set; }
	}
}
