using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for CircleGlyph.
	/// </summary>
	public class StateContactPointCircleGlyph : CircleGlyph
	{
		OffsetChangedHandler _PositionAlg;

		public StateContactPointCircleGlyph(Point centre, int radius, IGlyph parent, OffsetChangedHandler positionAlg)
			: base (centre, radius)
		{
			this.Parent = parent;
			_PositionAlg = positionAlg;
		}

		protected override void OnBeforeOffsetChanged(OffsetEventArgs offsetArgs)
		{
			if (_PositionAlg != null)
			{
				_PositionAlg (this, offsetArgs);
			}
		}

	}
}
