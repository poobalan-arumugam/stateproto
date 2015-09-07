using System;
using System.Drawing;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for IDrawStringContext.
	/// </summary>
	public interface IDrawStringContext
	{
		string Text { get; }
		int Thickness { get; }
		Color Color { get; }
		FontStyle FontStyle { get; }
	}
}
