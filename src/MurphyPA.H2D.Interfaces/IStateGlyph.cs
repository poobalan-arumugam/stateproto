using System;
using System.Drawing;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IStateGlyph.
	/// </summary>
	public interface IStateGlyph : IGroupGlyph
	{
		int CountParentDepth ();
		Color StateColor { get; }

		bool IsStartState { get; set; }
        bool IsFinalState { get; set; }
		bool IsOverriding { get; set; }
		string EntryAction { get; set; }
		string ExitAction { get; set; }
		string DoAction { get; set; }

		System.Collections.Specialized.StringCollection StateCommands { get; }
	}
}
