using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for ICompositeGlyph.
	/// </summary>
	public interface ICompositeGlyph : IGlyph
	{
		bool ContainsGlyph (IGlyph glyph);
	}
}
