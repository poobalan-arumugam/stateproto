using System;
using System.Drawing;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for OffsetEventArgs.
	/// </summary>
	public class OffsetEventArgs : EventArgs
	{
		public OffsetEventArgs(Point point)
		{
			_Point = point;
		}

		Point _Point;
		public Point Offset 
		{ 
			get 
			{
				return _Point;
			}
			set 
			{
				_Point = value;
			}
		}
	}
}
