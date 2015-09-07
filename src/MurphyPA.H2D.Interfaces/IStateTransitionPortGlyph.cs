using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IStateTransitionPortGlyph.
	/// </summary>
	public interface IStateTransitionPortGlyph : IGlyph
	{
		bool IsMultiPort { get; set; }
	}
}
