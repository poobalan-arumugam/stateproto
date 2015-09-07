using System;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for ConvertToCodeGlyphSorter.
	/// </summary>
	public class ConvertToCodeGlyphSorter : IComparer
	{
		#region IComparer Members
		public int Compare(object x, object y)
		{

			/// State ahead of Transition
			/// 
			/// State sorted in Parent Depth order
			/// Same Depth - sort by name
			/// 
			/// Transition sorted by event

			if (x == y) return 0;

			if (x is IStateGlyph && y is IStateGlyph)
			{
				IStateGlyph X = x as IStateGlyph;
				IStateGlyph Y = y as IStateGlyph;

				int overrideComp = X.IsOverriding.CompareTo (Y.IsOverriding);
				if (overrideComp == 0)
				{
					string xname = X.FullyQualifiedStateName;
					string yname = Y.FullyQualifiedStateName;
					return xname.CompareTo (yname);
				} 
				else 
				{
					return overrideComp;
				}
			} 
			else if (x is ITransitionGlyph && y is ITransitionGlyph)
			{
				ITransitionGlyph X = x as ITransitionGlyph;
				ITransitionGlyph Y = y as ITransitionGlyph;
				string xevent = X.Event != null ? X.Event : "";
				string yevent = Y.Event != null ? Y.Event : "";
				return xevent.CompareTo (yevent);
			} 
			else
			{
				if (x is IStateGlyph) // states to front of list
				{
					return -1;
				} 
				else 
				{
					return 1;
				}
			}
		}

		#endregion
	}
}
