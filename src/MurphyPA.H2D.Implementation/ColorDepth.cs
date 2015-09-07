using System;
using System.Drawing;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for ColorDepth.
	/// </summary>
	public class ColorDepth
	{
		public Color this [int index] 
		{
			get 
			{
				return _Colors [index % _Colors.Length];
			}
		}
		Color[] _Colors = new Color[] {Color.Blue, Color.DarkOrange, Color.DarkCyan, Color.Chocolate, Color.MidnightBlue, Color.BurlyWood, Color.DarkTurquoise, Color.BlueViolet, Color.DarkKhaki, Color.DarkSlateGray};
	}
}
