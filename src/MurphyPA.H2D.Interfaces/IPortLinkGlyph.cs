using System;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IPortLinkGlyph.
	/// </summary>
	public interface IPortLinkGlyph : IGroupGlyph
	{
		string FromPortName { get; set; }
		string ToPortName { get; set; }
		string SendIndex { get; set; }
	}
}
