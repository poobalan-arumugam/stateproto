using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for ITransitionContactPointGlyph.
	/// </summary>
	public interface ITransitionContactPointGlyph : IGlyph
	{
		TransitionContactEnd WhichEnd { get; }
	}
}
