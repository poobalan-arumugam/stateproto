using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for DrawStringContext.
	/// </summary>
	public class DrawStringContext : IDrawStringContext 
	{
		int _Thickness;
		string _Text;
		Color _Color;
		FontStyle _FontStyle;

		public DrawStringContext(string text, int thickness, Color color, FontStyle fontStyle)
		{
			_Thickness = thickness;
			_Text = text;
			_Color = color;
			_FontStyle = fontStyle;
		}

		#region IDrawStringContext Members

		public int Thickness
		{
			get
			{
				return _Thickness;
			}
		}

		public System.Drawing.Color Color
		{
			get
			{
				return _Color;
			}
		}

		public string Text
		{
			get
			{
				return _Text;
			}
		}

		public FontStyle FontStyle 
		{
			get 
			{
				return _FontStyle;
			}
		}

		#endregion
	}
}
