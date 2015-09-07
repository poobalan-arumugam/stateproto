using System;
using System.Collections;
using System.Drawing;

namespace MurphyPA.H2D.Interfaces
{
	/// <summary>
	/// Summary description for PointCollection.
	/// </summary>
	public class PointCollection : CollectionBase
	{
		public Point this [int index] 
		{
			get 
			{
				return (Point) (InnerList[index]);
			}
		}

		public void Add (Point point)
		{
			InnerList.Add (point);
		}

		public void Remove (Point point)
		{
			InnerList.Remove (point);
		}
	}
}
