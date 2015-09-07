using System;
using System.Drawing;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IComponentGlyph.
	/// </summary>
	public interface IComponentGlyph : IGroupGlyph
	{
		Color ComponentColor { get; }

		string TypeName { get; set; }
        bool IsMultiInstance { get; set; }
	}
}
