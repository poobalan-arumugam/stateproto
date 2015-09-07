using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IPortLinkContactPointGlyph.
	/// </summary>
	public interface IPortLinkContactPointGlyph : IGlyph
	{
		TransitionContactEnd WhichEnd { get; }
	}
}
