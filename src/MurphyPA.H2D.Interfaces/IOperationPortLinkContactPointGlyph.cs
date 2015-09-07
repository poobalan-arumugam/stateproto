using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IOperationPortLinkContactPointGlyph.
	/// </summary>
	public interface IOperationPortLinkContactPointGlyph : IGlyph
	{
		TransitionContactEnd WhichEnd { get; }
	}
}
