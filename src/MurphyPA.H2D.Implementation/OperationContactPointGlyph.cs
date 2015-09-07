using System;
using System.Drawing;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for OperationContactPointGlyph.
	/// </summary>
	public class OperationContactPointGlyph : SquareGlyph, IOperationContactPointGlyph
	{
		OffsetChangedHandler _PositionAlg;

		public OperationContactPointGlyph (Rectangle bounds, IGlyph parent, OffsetChangedHandler positionAlg)
			: base (bounds)
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
