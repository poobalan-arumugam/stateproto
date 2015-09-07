using System;
using System.Collections;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IGroupGlyph.
	/// </summary>
	public interface IGroupGlyph : ICompositeGlyph
	{
		IEnumerable ContactPoints { get; }
	}
}
